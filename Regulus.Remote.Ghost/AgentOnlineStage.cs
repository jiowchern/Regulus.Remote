using Regulus.Network;
using Regulus.Serialization;
using Regulus.Utility;
using System;

namespace Regulus.Remote.Ghost
{
    public partial class Agent
    {

        private class GhostSerializer : IGhostRequest
        {


            private static readonly object _LockRequest = new object();

            private static readonly object _LockResponse = new object();


            private readonly PackageReader<ResponsePackage> _Reader;

            private readonly Regulus.Collection.Queue<ResponsePackage> _Receives;

            private readonly Regulus.Collection.Queue<RequestPackage> _Sends;

            private readonly PackageWriter<RequestPackage> _Writer;


            public static int RequestQueueCount { get; private set; }

            public static int ResponseQueueCount { get; private set; }

            public GhostSerializer(ISerializer serializer)
            {
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
                lock (GhostSerializer._LockRequest)
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

            public void Start(IStreamable peer)
            {
                Singleton<Log>.Instance.WriteInfo("Agent online enter.");

                /*todo : Singleton<Log>.Instance.WriteInfo(
					string.Format(
						"Agent Socket Local {0} Remote {1}.", 
						_Peer.LocalEndPoint, 
						_Peer.RemoteEndPoint));*/

                _ReaderStart(peer);
                _WriterStart(peer);



            }

            public void Stop()
            {


                _WriterStop();
                _ReaderStop();



                Singleton<Log>.Instance.WriteInfo("Agent online leave.");
            }

            void _Update()
            {
                _Process();
            }

            private void _ReceivePackage(ResponsePackage package)
            {
                lock (GhostSerializer._LockResponse)
                {
                    _Receives.Enqueue(package);
                    GhostSerializer.ResponseQueueCount++;
                }
            }

            private void _Process()
            {
                lock (GhostSerializer._LockResponse)
                {
                    ResponsePackage[] pkgs = _Receives.DequeueAll();
                    GhostSerializer.ResponseQueueCount -= pkgs.Length;

                    foreach (ResponsePackage pkg in pkgs)
                    {
                        _ResponseEvent(pkg.Code, pkg.Data);
                    }
                }


                RequestPackage[] sends = _SendsPop();
                if (sends.Length > 0)
                    _Writer.Push(sends);


            }

            private void _WriterStart(IStreamable peer)
            {
                _Writer.ErrorEvent += _Disable;

                _Writer.Start(peer);
            }

            private void _WriterStop()
            {
                _Writer.ErrorEvent -= _Disable;

                _Writer.Stop();
            }

            private RequestPackage[] _SendsPop()
            {
                lock (GhostSerializer._LockRequest)
                {
                    RequestPackage[] pkg = _Sends.DequeueAll();
                    GhostSerializer.RequestQueueCount -= pkg.Length;
                    return pkg;
                }
            }

            private void _ReaderStart(IStreamable peer)
            {
                _Reader.DoneEvent += _ReceivePackage;

                _Reader.ErrorEvent += _Disable;
                _Reader.Start(peer);
            }

            private void _Disable()
            {

            }

            private void _ReaderStop()
            {
                _Reader.DoneEvent -= _ReceivePackage;
                _Reader.ErrorEvent -= _Disable;
                _Reader.Stop();
            }

            public void Update()
            {
                _Update();
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
