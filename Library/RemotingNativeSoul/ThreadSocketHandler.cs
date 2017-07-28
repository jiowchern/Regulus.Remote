using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Regulus.Network;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Remoting.Soul.Native
{
    internal class ThreadSocketHandler
    {
        private readonly ThreadCoreHandler _CoreHandler;
        private readonly IProtocol _Protocol;
        private readonly IPeerServer _Server;

        private readonly PeerSet _Peers;

        private readonly int _Port;

        private readonly Queue<IPeer> _Sockets;

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

            _Sockets = new Queue<IPeer>();

            _Peers = new PeerSet();

            _Spin = new PowerRegulator();
            _AutoPowerRegulator = new AutoPowerRegulator(_Spin);

            _Server = new Regulus.Network.RudpServer(new UdpSocket());
        }

        public void DoWork(object obj)
        {
            Singleton<Log>.Instance.WriteInfo("server peer launch");
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
                                string.Format("peer accept Remote {0} Local {1} .", socket.RemoteEndPoint, socket.LocalEndPoint));
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
            Singleton<Log>.Instance.WriteInfo("server peer shutdown");
        }

        private void _Accept(IPeer peer)
        {
            lock (_Sockets)
            {
                _Sockets.Enqueue(peer);
            }
        }
        

        public void Stop()
        {
            _Run = false;
        }
    }
}