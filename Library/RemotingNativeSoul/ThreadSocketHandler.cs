using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Regulus.Network;
using Regulus.Utility;

namespace Regulus.Remoting.Soul.Native
{
    internal class ThreadSocketHandler
    {
        private readonly ThreadCoreHandler _CoreHandler;
        private readonly IProtocol _Protocol;
        private readonly ISocketServer _Server;

        private readonly PeerSet _Peers;

        private readonly int _Port;

        private readonly Queue<ISocket> _Sockets;

        // ParallelUpdate _Peers;
        private readonly PowerRegulator _Spin;

        private readonly AutoPowerRegulator _AutoPowerRegulator;

        private volatile bool _Run;
        

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

        public ThreadSocketHandler(int port, ThreadCoreHandler core_handler, IProtocol protocol)
        {
            _CoreHandler = core_handler;
            _Protocol = protocol;
            _Port = port;

            _Sockets = new Queue<ISocket>();

            _Peers = new PeerSet();

            _Spin = new PowerRegulator();
            _AutoPowerRegulator = new AutoPowerRegulator(_Spin);

            _Server = new Regulus.Network.RudpServer();
        }

        public void DoWork(object obj)
        {
            Singleton<Log>.Instance.WriteInfo("server socket launch");
            var are = (AutoResetEvent)obj;
            _Run = true;

            _Server.AcceptEvent += _Accept;
            _Server.Bind(_Port);

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
                                string.Format("socket accept Remote {0} Local {1} .", socket.RemoteEndPoint, socket.LocalEndPoint));
                            var peer = new Peer(socket , _Protocol);

                            _Peers.Join(peer);

                            _CoreHandler.Push(peer.Binder, peer.Handler);
                        }
                    }
                }

                _AutoPowerRegulator.Operate();
            }

            _Peers.Release();


            _Server.AcceptEvent -= _Accept;
            _Server.Close();


            are.Set();
            Singleton<Log>.Instance.WriteInfo("server socket shutdown");
        }

        private void _Accept(ISocket socket)
        {
            lock (_Sockets)
            {
                _Sockets.Enqueue(socket);
            }
        }
        

        public void Stop()
        {
            _Run = false;
        }
    }
}