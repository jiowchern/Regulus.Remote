using Regulus.Network;
using Regulus.Utility;

namespace Regulus.Remote.Soul
{
    internal class ThreadSocketHandler
    {

        private readonly IProtocol _Protocol;
        private readonly IListenable _Server;

        private readonly PeerSet _Peers;

        private readonly int _Port;

        private readonly System.Collections.Concurrent.ConcurrentQueue<IStreamable> _Sockets;


        private readonly PowerRegulator _Spin;

        private readonly AutoPowerRegulator _AutoPowerRegulator;

        private volatile bool _Run;

        readonly System.Threading.Tasks.Task _Task;

        public event System.Action<IBinder> BinderEvent;


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

        public ThreadSocketHandler(int port, IProtocol protocol, IListenable server)
        {

            _Protocol = protocol;
            _Port = port;

            _Sockets = new System.Collections.Concurrent.ConcurrentQueue<IStreamable>();

            _Peers = new PeerSet();

            _Spin = new PowerRegulator();
            _AutoPowerRegulator = new AutoPowerRegulator(_Spin);

            _Server = server;

            _Task = new System.Threading.Tasks.Task(this._DoWork, System.Threading.Tasks.TaskCreationOptions.LongRunning);
        }

        void _DoWork()
        {
            Singleton<Log>.Instance.WriteInfo("server launch.");
            _Run = true;

            _Server.AcceptEvent += _Accept;
            _Server.Bind(_Port);

            while (_Run)
            {
                IStreamable socket;
                if (_Sockets.TryDequeue(out socket))
                {
                    /*todo : Singleton<Log>.Instance.WriteInfo(
                                string.Format("accept Remote {0} Local {1} .", socket.RemoteEndPoint, socket.LocalEndPoint));*/
                    User peer = new User(socket, _Protocol);

                    _Peers.Join(peer);

                    BinderEvent(peer.Binder);
                }
                _Peers.RemoveInvalidPeers();

                _AutoPowerRegulator.Operate();
            }

            _Peers.Release();


            _Server.AcceptEvent -= _Accept;
            _Server.Close();


            Singleton<Log>.Instance.WriteInfo("server shutdown.");
        }

        private void _Accept(IStreamable peer)
        {
            lock (_Sockets)
            {
                _Sockets.Enqueue(peer);
            }
        }

        internal void Start()
        {
            _Task.Start();
        }

        public void Stop()
        {
            _Run = false;
            _Task.Wait();
        }
    }
}