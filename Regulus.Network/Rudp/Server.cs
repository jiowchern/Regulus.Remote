using Regulus.Utility;
using System;
using System.Threading;

namespace Regulus.Network.Rudp
{
    public class Listener : IListenable
    {
        private readonly ISocket _Socket;
        private readonly ITime _Time;
        private readonly Host _Host;
        private volatile bool _Enable;
        private event Action<IPeer> _AcceptEvent;

        public Listener(ISocket Socket)
        {
            _Host = new Host(Socket, Socket);
            _Socket = Socket;
            _Time = new Time();
        }

        event Action<IPeer> IListenable.AcceptEvent
        {
            add { _AcceptEvent += value; }
            remove { _AcceptEvent -= value; }
        }

        void IListenable.Bind(int Port)
        {
            _Socket.Bind(Port);
            _Host.AcceptEvent += Accept;
            _Enable = true;
            ThreadPool.QueueUserWorkItem(Run, state: null);
        }

        private void Run(object State)
        {
            Updater<Timestamp> updater = new Updater<Timestamp>();
            updater.Add(_Host);

            AutoPowerRegulator wait = new AutoPowerRegulator(new PowerRegulator());
            while (_Enable)
            {
                _Time.Sample();
                updater.Working(new Timestamp(_Time.Now, _Time.Delta));
                wait.Operate();
            }

            updater.Shutdown();

        }

        private void Accept(Regulus.Network.Socket rudp_socket)
        {
            _AcceptEvent(new Peer(rudp_socket));
        }

        void IListenable.Close()
        {
            _Socket.Close();
            _Host.AcceptEvent -= Accept;
            _Enable = false;
        }
    }
}
