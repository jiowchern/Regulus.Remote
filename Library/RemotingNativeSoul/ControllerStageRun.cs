using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;


using Regulus.Utility;


using Console = Regulus.Utility.Console;

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
			foreach(var up in _GetObjectSet())
			{
				_Update(up);
			}
		}

		private void _Update(IUpdatable updater)
		{
			var result = false;

			result = updater.Update();

			if(result == false)
			{
				Remove(updater);
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
			get { return _Spin.FPS; }
		}

		public float Power
		{
			get { return _Spin.Power; }
		}

		public ThreadCoreHandler(ICore core)
		{
			if(core == null)
			{
				throw new ArgumentNullException();
			}

			_Core = core;

			_RequesterHandlers = new Updater();
			_Spin = new PowerRegulator();
			_Binders = new Queue<ISoulBinder>();
		}

		public void DoWork(object obj)
		{
			Singleton<Log>.Instance.WriteInfo("server core launch");
			_Run = true;
			_Core.Launch();

			while(_Run)
			{
				if(_Binders.Count > 0)
				{
					lock(_Binders)
					{
						while(_Binders.Count > 0)
						{
							var provider = _Binders.Dequeue();
							_Core.AssignBinder(provider);
						}
					}
				}

				_Core.Update();
				_RequesterHandlers.Working();
				_Spin.Operate(Peer.TotalResponse);
			}

			_Core.Shutdown();

			Singleton<Log>.Instance.WriteInfo("server core shutdown");
		}

		public void Stop()
		{
			_Run = false;
		}

		internal void Push(ISoulBinder soulBinder, IUpdatable handler)
		{
			_RequesterHandlers.Add(handler);

			lock(_Binders)
			{
				_Binders.Enqueue(soulBinder);
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
			get { return _Spin.FPS; }
		}

		public float Power
		{
			get { return _Spin.Power; }
		}

		public int PeerCount
		{
			get { return _Peers.Count; }
		}

		public ThreadSocketHandler(int port, ThreadCoreHandler core_handler)
		{
			_CoreHandler = core_handler;
			_Port = port;

			_Sockets = new Queue<Socket>();

			_Peers = new PeerSet();

			_Spin = new PowerRegulator();
		}

		public void DoWork(object obj)
		{
			Singleton<Log>.Instance.WriteInfo("server socket launch");
			var are = (AutoResetEvent)obj;
			_Run = true;

			_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_Socket.NoDelay = true;

			// _Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
			_Socket.Bind(new IPEndPoint(IPAddress.Any, _Port));
			_Socket.Listen(5);
			_Socket.BeginAccept(_Accept, null);

			while(_Run)
			{
				if(_Sockets.Count > 0)
				{
					lock(_Sockets)
					{
						while(_Sockets.Count > 0)
						{
							var socket = _Sockets.Dequeue();

							Singleton<Log>.Instance.WriteInfo(
								string.Format("socket accept Remot {0} Local {1} .", socket.RemoteEndPoint, socket.LocalEndPoint));
							var peer = new Peer(socket);

							_Peers.Join(peer);

							_CoreHandler.Push(peer.Binder, peer.Handler);
						}
					}
				}

				_Spin.Operate(Peer.TotalRequest);
			}

			_Peers.Release();

			if(_Socket.Connected)
			{
				_Socket.Shutdown(SocketShutdown.Both);
			}

			_Socket.Close();

			are.Set();
			Singleton<Log>.Instance.WriteInfo("server socket shutdown");
		}

		private void _Accept(IAsyncResult ar)
		{
			try
			{
				var socket = _Socket.EndAccept(ar);
				lock(_Sockets)
				{
					_Sockets.Enqueue(socket);
				}

				_Socket.BeginAccept(_Accept, null);
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
			catch(SocketException se)
			{
				Singleton<Log>.Instance.WriteInfo(se.ToString());
			}
			catch(ObjectDisposedException ode)
			{
				Singleton<Log>.Instance.WriteInfo(ode.ToString());
			}
			catch(InvalidOperationException ioe)
			{
				Singleton<Log>.Instance.WriteInfo(ioe.ToString());
			}
			catch(Exception e)
			{
				Singleton<Log>.Instance.WriteInfo(e.ToString());
			}
		}

		public void Stop()
		{
			_Run = false;
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
			_View = viewer;
			_Command = command;

			_Server = new Server(core, port);
			_Launcher = new Launcher();
			_Launcher.Push(_Server);
		}

		void IStage.Enter()
		{
			_Launcher.Launch();
			_Command.Register(
				"FPS", 
				() =>
				{
					_View.WriteLine("PeerFPS:" + _Server.PeerFPS);
					_View.WriteLine("PeerCount:" + _Server.PeerCount);
					_View.WriteLine("CoreFPS:" + _Server.CoreFPS);

					_View.WriteLine(string.Format("PeerUsage:{0:0.00%}", _Server.PeerUsage));
					_View.WriteLine(string.Format("CoreUsage:{0:0.00%}", _Server.CoreUsage));

					_View.WriteLine(
						"\nTotalReadBytes:"
						+ string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Read.TotalBytes));
					_View.WriteLine(
						"TotalWriteBytes:"
						+ string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Write.TotalBytes));
					_View.WriteLine(
						"\nSecondReadBytes:"
						+ string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Read.SecondBytes));
					_View.WriteLine(
						"SecondWriteBytes:"
						+ string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Write.SecondBytes));
					_View.WriteLine("\nRequest Queue:" + Peer.TotalRequest);
					_View.WriteLine("Response Queue:" + Peer.TotalResponse);
				});
			_Command.Register("Shutdown", _ShutdownEvent);
		}

		void IStage.Leave()
		{
			_Launcher.Shutdown();

			_Command.Unregister("Shutdown");
			_Command.Unregister("FPS");
		}

		void IStage.Update()
		{
		}

		private void _ShutdownEvent()
		{
			ShutdownEvent();
		}
	}
}
