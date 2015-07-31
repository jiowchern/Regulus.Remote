// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerStageRun.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ParallelUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Regulus.Utility;

using Console = Regulus.Utility.Console;

#endregion

namespace Regulus.Remoting.Soul.Native
{
	internal class ParallelUpdate : Launcher<IUpdatable>
	{
		public void Update()
		{
			/*var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

            Parallel.ForEach(base._GetObjectSet(), (updater) => 
            {
                try
                {
                    _Update(updater);
                }
                catch (Exception e) { exceptions.Enqueue(e); }
            });

            if (exceptions.Count > 0) 
                throw new AggregateException(exceptions);*/
			foreach (var up in this._GetObjectSet())
			{
				this._Update(up);
			}
		}

		private void _Update(IUpdatable updater)
		{
			var result = false;

			result = updater.Update();

			if (result == false)
			{
				this.Remove(updater);
			}
		}
	}

	internal class ThreadCoreHandler
	{
		private readonly Queue<ISoulBinder> _Binders;

		private readonly ICore _Core;

		private readonly Updater _RequesterHandlers;

		private readonly PowerRegulator _Spin;

		private volatile bool _Run;

		public int FPS
		{
			get { return this._Spin.FPS; }
		}

		public float Power
		{
			get { return this._Spin.Power; }
		}

		public ThreadCoreHandler(ICore core)
		{
			if (core == null)
			{
				throw new ArgumentNullException();
			}

			this._Core = core;

			this._RequesterHandlers = new Updater();
			this._Spin = new PowerRegulator();
			this._Binders = new Queue<ISoulBinder>();
		}

		public void DoWork(object obj)
		{
			Singleton<Log>.Instance.WriteInfo("server core launch");
			this._Run = true;
			this._Core.Launch();

			while (this._Run)
			{
				if (this._Binders.Count > 0)
				{
					lock (this._Binders)
					{
						while (this._Binders.Count > 0)
						{
							var provider = this._Binders.Dequeue();
							this._Core.AssignBinder(provider);
						}
					}
				}

				this._Core.Update();
				this._RequesterHandlers.Working();
				this._Spin.Operate(Peer.TotalResponse);
			}

			this._Core.Shutdown();

			Singleton<Log>.Instance.WriteInfo("server core shutdown");
		}

		public void Stop()
		{
			this._Run = false;
		}

		internal void Push(ISoulBinder soulBinder, IUpdatable handler)
		{
			this._RequesterHandlers.Add(handler);

			lock (this._Binders)
			{
				this._Binders.Enqueue(soulBinder);
			}
		}
	}


	internal class ThreadSocketHandler
	{
		private readonly ThreadCoreHandler _CoreHandler;

		private readonly PeerSet _Peers;

		private readonly int _Port;

		private readonly Queue<Socket> _Sockets;

		// ParallelUpdate _Peers;
		private readonly PowerRegulator _Spin;

		private volatile bool _Run;

		private Socket _Socket;

		public int FPS
		{
			get { return this._Spin.FPS; }
		}

		public float Power
		{
			get { return this._Spin.Power; }
		}

		public int PeerCount
		{
			get { return this._Peers.Count; }
		}

		public ThreadSocketHandler(int port, ThreadCoreHandler core_handler)
		{
			this._CoreHandler = core_handler;
			this._Port = port;

			this._Sockets = new Queue<Socket>();

			this._Peers = new PeerSet();

			this._Spin = new PowerRegulator();
		}

		public void DoWork(object obj)
		{
			Singleton<Log>.Instance.WriteInfo("server socket launch");
			var are = (AutoResetEvent)obj;
			this._Run = true;

			this._Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._Socket.NoDelay = true;

			// _Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
			this._Socket.Bind(new IPEndPoint(IPAddress.Any, this._Port));
			this._Socket.Listen(5);
			this._Socket.BeginAccept(this._Accept, null);

			while (this._Run)
			{
				if (this._Sockets.Count > 0)
				{
					lock (this._Sockets)
					{
						while (this._Sockets.Count > 0)
						{
							var socket = this._Sockets.Dequeue();

							Singleton<Log>.Instance.WriteInfo(
								string.Format("socket accept Remot {0} Local {1} .", socket.RemoteEndPoint, socket.LocalEndPoint));
							var peer = new Peer(socket);

							this._Peers.Join(peer);

							this._CoreHandler.Push(peer.Binder, peer.Handler);
						}
					}
				}

				this._Spin.Operate(Peer.TotalRequest);
			}

			this._Peers.Release();

			if (this._Socket.Connected)
			{
				this._Socket.Shutdown(SocketShutdown.Both);
			}

			this._Socket.Close();

			are.Set();
			Singleton<Log>.Instance.WriteInfo("server socket shutdown");
		}

		private void _Accept(IAsyncResult ar)
		{
			try
			{
				var socket = this._Socket.EndAccept(ar);
				lock (this._Sockets)
				{
					this._Sockets.Enqueue(socket);
				}

				this._Socket.BeginAccept(this._Accept, null);
			}

				// System.ArgumentNullException:
				// asyncResult 為 null。
				// System.ArgumentException:
				// asyncResult 不是透過呼叫 System.Net.Sockets.Socket.BeginAccept(System.AsyncCallback,System.Object)
				// 所建立。
				// System.Net.Sockets.SocketException:
				// 嘗試存取通訊端時發生錯誤。如需詳細資訊，請參閱備註章節。
				// System.ObjectDisposedException:
				// System.Net.Sockets.Socket 已經關閉。
				// System.InvalidOperationException:
				// 先前已呼叫 System.Net.Sockets.Socket.EndAccept(System.IAsyncResult) 方法。
			catch (SocketException se)
			{
				Singleton<Log>.Instance.WriteInfo(se.ToString());
			}
			catch (ObjectDisposedException ode)
			{
				Singleton<Log>.Instance.WriteInfo(ode.ToString());
			}
			catch (InvalidOperationException ioe)
			{
				Singleton<Log>.Instance.WriteInfo(ioe.ToString());
			}
			catch (Exception e)
			{
				Singleton<Log>.Instance.WriteInfo(e.ToString());
			}
		}

		public void Stop()
		{
			this._Run = false;
		}
	}

	internal class StageRun : IStage
	{
		public event Action ShutdownEvent;

		private readonly Command _Command;

		private readonly Launcher _Launcher;

		private readonly Server _Server;

		private readonly Console.IViewer _View;

		public StageRun(ICore core, Command command, int port, Console.IViewer viewer)
		{
			this._View = viewer;
			this._Command = command;

			this._Server = new Server(core, port);
			this._Launcher = new Launcher();
			this._Launcher.Push(this._Server);
		}

		void IStage.Enter()
		{
			this._Launcher.Launch();
			this._Command.Register("FPS", () =>
			{
				this._View.WriteLine("PeerFPS:" + this._Server.PeerFPS);
				this._View.WriteLine("PeerCount:" + this._Server.PeerCount);
				this._View.WriteLine("CoreFPS:" + this._Server.CoreFPS);

				this._View.WriteLine(string.Format("PeerUsage:{0:0.00%}", this._Server.PeerUsage));
				this._View.WriteLine(string.Format("CoreUsage:{0:0.00%}", this._Server.CoreUsage));

				this._View.WriteLine("\nTotalReadBytes:"
				                     + string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Read.TotalBytes));
				this._View.WriteLine("TotalWriteBytes:"
				                     + string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Write.TotalBytes));
				this._View.WriteLine("\nSecondReadBytes:"
				                     + string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Read.SecondBytes));
				this._View.WriteLine("SecondWriteBytes:"
				                     + string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Write.SecondBytes));
				this._View.WriteLine("\nRequest Queue:" + Peer.TotalRequest);
				this._View.WriteLine("Response Queue:" + Peer.TotalResponse);
			});
			this._Command.Register("Shutdown", this._ShutdownEvent);
		}

		void IStage.Leave()
		{
			this._Launcher.Shutdown();

			this._Command.Unregister("Shutdown");
			this._Command.Unregister("FPS");
		}

		void IStage.Update()
		{
		}

		private void _ShutdownEvent()
		{
			this.ShutdownEvent();
		}
	}
}