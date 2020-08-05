using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Regulus.Network;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Remote.Ghost
{
	public partial class Agent
	{

		private class GhostSerializer : IUpdatable, IGhostRequest
        {
			public event Action DoneFromServerEvent;

			private static readonly object _LockRequest = new object();

			private static readonly object _LockResponse = new object();			
			
			
			private readonly PackageReader<ResponsePackage> _Reader;

			private readonly Regulus.Collection.Queue<ResponsePackage> _Receives;

			private readonly Regulus.Collection.Queue<RequestPackage> _Sends;

			private readonly PackageWriter<RequestPackage> _Writer;
			
			private volatile bool _Enable;

			private readonly IPeer _Peer;

			public static int RequestQueueCount { get; private set; }

			public static int ResponseQueueCount { get; private set; }

			public GhostSerializer(IPeer peer, ISerializer serializer )
			{
                
                
				
				_Peer = peer;
				_Reader = new PackageReader<ResponsePackage>(serializer);
				_Writer = new PackageWriter<RequestPackage>(serializer);
				_Sends = new Collection.Queue<RequestPackage>();
				_Receives = new Collection.Queue<ResponsePackage>();
				
			}

			event Action<ServerToClientOpCode, byte[]> _ResponseEvent;

			event Action<ServerToClientOpCode, byte[]> IGhostRequest.ResponseEvent
            {
                add
                {
					_ResponseEvent += value;

				}

                remove
                {
					_ResponseEvent -= value;
				}
            }

            void IGhostRequest.Request(ClientToServerOpCode code, byte[] args)
			{
				lock(GhostSerializer._LockRequest)
				{
					_Sends.Enqueue(
						new RequestPackage()
						{
							Data = args, 
							Code = code
						});
					GhostSerializer.RequestQueueCount++;
				}
			}

			void IBootable.Launch()
			{
				Singleton<Log>.Instance.WriteInfo("Agent online enter.");

				Singleton<Log>.Instance.WriteInfo(
					string.Format(
						"Agent Socket Local {0} Remote {1}.", 
						_Peer.LocalEndPoint, 
						_Peer.RemoteEndPoint));
				


				
				_Enable = true;
				_ReaderStart();
				_WriterStart();

				
				
			}

			void IBootable.Shutdown()
			{
				

				_WriterStop();
				_ReaderStop();

				_Peer.Close();
				
				Singleton<Log>.Instance.WriteInfo("Agent online leave.");
			}

			void _Update()
			{
				if(_Enable == false)
				{
					DoneFromServerEvent();
				}
				else
				{
					_Process();
				}
			}

			private void _ReceivePackage(ResponsePackage package)
			{
				lock(GhostSerializer._LockResponse)
				{
					_Receives.Enqueue(package);
					GhostSerializer.ResponseQueueCount++;
				}
			}

			private void _Process()
			{
				lock(GhostSerializer._LockResponse)
				{
					var pkgs = _Receives.DequeueAll();
					GhostSerializer.ResponseQueueCount -= pkgs.Length;

					foreach(var pkg in pkgs)
					{
						_ResponseEvent(pkg.Code , pkg.Data);						
					}
				}


			    var sends = _SendsPop();
                if(sends.Length > 0)
                    _Writer.Push(sends);

                
			}

			private void _WriterStart()
			{
				_Writer.ErrorEvent += _Disable;
				
				_Writer.Start(_Peer);
			}

			private void _WriterStop()
			{
				_Writer.ErrorEvent -= _Disable;
				
				_Writer.Stop();
			}

			private RequestPackage[] _SendsPop()
			{
				lock(GhostSerializer._LockRequest)
				{
					var pkg = _Sends.DequeueAll();
					GhostSerializer.RequestQueueCount -= pkg.Length;
					return pkg;
				}
			}

			private void _ReaderStart()
			{
				_Reader.DoneEvent += _ReceivePackage;

				_Reader.ErrorEvent += _Disable;
				_Reader.Start(_Peer);
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

            bool IUpdatable.Update()
            {
				_Update();
				return _Enable;
            }
        }

		/// <summary>
		///     請求的封包
		/// </summary>
		public static int RequestPackages
		{
			get { return GhostSerializer.RequestQueueCount; }
		}

		/// <summary>
		///     回應的封包
		/// </summary>
		public static int ResponsePackages
		{
			get { return GhostSerializer.ResponseQueueCount; }
		}
	}
}
