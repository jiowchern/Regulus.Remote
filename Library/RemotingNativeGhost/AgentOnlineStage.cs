// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentOnlineStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Agent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Net.Sockets;

using Regulus.Utility;

#endregion

namespace Regulus.Remoting.Ghost.Native
{
	public partial class Agent
	{
		/// <summary>
		///     請求的封包
		/// </summary>
		public static int RequestPackages
		{
			get { return OnlineStage.RequestQueueCount; }
		}

		/// <summary>
		///     回應的封包
		/// </summary>
		public static int ResponsePackages
		{
			get { return OnlineStage.ResponseQueueCount; }
		}

		private class OnlineStage : IStage, IGhostRequest
		{
			public event Action DoneFromServerEvent;

			private static readonly object _LockRequest = new object();

			private static readonly object _LockResponse = new object();

			public static readonly int LowFps = 30;

			private readonly AgentCore _Core;

			private readonly PackageReader _Reader;

			private readonly PackageQueue _Receives;

			private readonly PackageQueue _Sends;

			private readonly PackageWriter _Writer;

			private volatile bool _Enable;

			private Socket _Socket;

			public static int RequestQueueCount { get; private set; }

			public static int ResponseQueueCount { get; private set; }

			public OnlineStage(Socket socket, AgentCore core)
			{
				this._Core = core;

				_Socket = socket;
				_Reader = new PackageReader();
				_Writer = new PackageWriter(OnlineStage.LowFps);
				_Sends = new PackageQueue();
				_Receives = new PackageQueue();
			}

			void IGhostRequest.Request(byte code, Dictionary<byte, byte[]> args)
			{
				lock (OnlineStage._LockRequest)
				{
					_Sends.Enqueue(new Package
					{
						Args = args, 
						Code = code
					});
					OnlineStage.RequestQueueCount++;
				}
			}

			void IStage.Enter()
			{
				Singleton<Log>.Instance.WriteInfo("Agent online enter.");

				Singleton<Log>.Instance.WriteInfo(string.Format("Agent Socket Local {0} Remot {1}.", this._Socket.LocalEndPoint, 
					this._Socket.RemoteEndPoint));
				_Core.Initial(this);
				_Enable = true;
				_ReaderStart();
				_WriterStart();
			}

			void IStage.Leave()
			{
				_WriterStop();
				_ReaderStop();

				if (_Socket != null)
				{
					if (_Socket.Connected)
					{
						_Socket.Shutdown(SocketShutdown.Both);
					}

					_Socket.Close();
					_Socket = null;
				}

				_Core.Finial();
				Singleton<Log>.Instance.WriteInfo("Agent online leave.");
			}

			void IStage.Update()
			{
				if (_Enable == false)
				{
					DoneFromServerEvent();
				}
				else
				{
					_Process(_Core);
				}
			}

			private void _ReceivePackage(Package package)
			{
				lock (OnlineStage._LockResponse)
				{
					_Receives.Enqueue(package);
					OnlineStage.ResponseQueueCount++;
				}
			}

			private void _Process(AgentCore core)
			{
				lock (OnlineStage._LockResponse)
				{
					var pkgs = _Receives.DequeueAll();
					OnlineStage.ResponseQueueCount -= pkgs.Length;

					foreach (var pkg in pkgs)
					{
						core.OnResponse(pkg.Code, pkg.Args);
					}
				}
			}

			private void _WriterStart()
			{
				_Writer.ErrorEvent += _Disable;
				_Writer.CheckSourceEvent += _SendsPop;
				_Writer.Start(_Socket);
			}

			private void _WriterStop()
			{
				_Writer.ErrorEvent -= _Disable;
				_Writer.CheckSourceEvent -= _SendsPop;
				_Writer.Stop();
			}

			private Package[] _SendsPop()
			{
				lock (OnlineStage._LockRequest)
				{
					var pkg = _Sends.DequeueAll();
					OnlineStage.RequestQueueCount -= pkg.Length;
					return pkg;
				}
			}

			private void _ReaderStart()
			{
				_Reader.DoneEvent += _ReceivePackage;

				_Reader.ErrorEvent += _Disable;
				_Reader.Start(_Socket);
			}

			private void _Disable()
			{
				_Enable = false;
			}

			private void _ReaderStop()
			{
				_Reader.DoneEvent -= _ReceivePackage;
				_Reader.ErrorEvent -= _Disable;
				_Reader.Stop();
			}
		}
	}
}