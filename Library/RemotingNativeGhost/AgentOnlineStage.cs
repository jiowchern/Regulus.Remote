using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Regulus.Network;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Remoting.Ghost.Native
{
	public partial class Agent
	{
		private class OnlineStage : IStage, IGhostRequest
		{
			public event Action DoneFromServerEvent;

			private static readonly object _LockRequest = new object();

			private static readonly object _LockResponse = new object();			

			private readonly AgentCore _Core;

			private readonly PackageReader<ResponsePackage> _Reader;

			private readonly Regulus.Collection.Queue<ResponsePackage> _Receives;

			private readonly Regulus.Collection.Queue<RequestPackage> _Sends;

			private readonly PackageWriter<RequestPackage> _Writer;

			private volatile bool _Enable;

			private ISocket _Socket;

			public static int RequestQueueCount { get; private set; }

			public static int ResponseQueueCount { get; private set; }

			public OnlineStage(ISocket socket, AgentCore core , ISerializer serializer)
			{
                
                _Core = core;

				_Socket = socket;
				_Reader = new PackageReader<ResponsePackage>(serializer);
				_Writer = new PackageWriter<RequestPackage>(serializer);
				_Sends = new Collection.Queue<RequestPackage>();
				_Receives = new Collection.Queue<ResponsePackage>();
			}

			void IGhostRequest.Request(ClientToServerOpCode code, byte[] args)
			{
				lock(OnlineStage._LockRequest)
				{
					_Sends.Enqueue(
						new RequestPackage()
						{
							Data = args, 
							Code = code
						});
					OnlineStage.RequestQueueCount++;
				}
			}

			void IStage.Enter()
			{
				Singleton<Log>.Instance.WriteInfo("Agent online enter.");

				Singleton<Log>.Instance.WriteInfo(
					string.Format(
						"Agent Socket Local {0} Remote {1}.", 
						_Socket.LocalEndPoint, 
						_Socket.RemoteEndPoint));
				_Core.Initial(this);
				_Enable = true;
				_ReaderStart();
				_WriterStart();
			}

			void IStage.Leave()
			{
				_WriterStop();
				_ReaderStop();

				if(_Socket != null)
				{					
					_Socket.Close();
					_Socket = null;
				}

				_Core.Finial();
				Singleton<Log>.Instance.WriteInfo("Agent online leave.");
			}

			void IStage.Update()
			{
				if(_Enable == false)
				{
					DoneFromServerEvent();
				}
				else
				{
					_Process(_Core);
				}
			}

			private void _ReceivePackage(ResponsePackage package)
			{
				lock(OnlineStage._LockResponse)
				{
					_Receives.Enqueue(package);
					OnlineStage.ResponseQueueCount++;
				}
			}

			private void _Process(AgentCore core)
			{
				lock(OnlineStage._LockResponse)
				{
					var pkgs = _Receives.DequeueAll();
					OnlineStage.ResponseQueueCount -= pkgs.Length;

					foreach(var pkg in pkgs)
					{
						core.OnResponse(pkg.Code, pkg.Data);
					}
				}


			    var sends = _SendsPop();
                if(sends.Length > 0)
                    _Writer.Push(sends);

                
			}

			private void _WriterStart()
			{
				_Writer.ErrorEvent += _Disable;
				
				_Writer.Start(_Socket);
			}

			private void _WriterStop()
			{
				_Writer.ErrorEvent -= _Disable;
				
				_Writer.Stop();
			}

			private RequestPackage[] _SendsPop()
			{
				lock(OnlineStage._LockRequest)
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
	}
}
