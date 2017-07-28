using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Regulus.Network.RUDP;

using Regulus.Utility;

namespace Regulus.Network
{
    public class RudpServer : IPeerServer
    {
        private readonly ISocket _Socket;
        private readonly ITime _Time;
        private Host _Host;
        private bool _Enable;
        private event Action<IPeer> _AcctpeEvent;

        public RudpServer(ISocket socket)
        {
            _Host = new Host(socket,socket);
            _Socket = socket;
            _Time = new Time();
        }

        event Action<IPeer> IPeerServer.AcceptEvent
        {
            add { _AcctpeEvent += value; }
            remove { _AcctpeEvent -= value; }
        }

        void IPeerServer.Bind(int port)
        {
            _Socket.Bind(port);
            _Host.AcceptEvent += _Accept;
            _Enable = true;
            ThreadPool.QueueUserWorkItem(_Run, null);
        }

        private void _Run(object state)
        {
            var updater = new Regulus.Utility.Updater<Timestamp>();
            updater.Add(_Host);
            
            //var regulator = new Regulus.Utility.AutoPowerRegulator(new PowerRegulator());
            while (_Enable)
            {
                _Time.Sample();
                updater.Working(new Timestamp(_Time.Now, _Time.Delta));
                //regulator.Operate();
            }

            updater.Shutdown();

        }

        private void _Accept(IRudpPeer rudpPeer)
        {
            _AcctpeEvent(new RudpPeer(rudpPeer));
        }

        void IPeerServer.Close()
        {
            _Socket.Close();
            _Host.AcceptEvent -= _Accept;
            _Enable = false;
        }
    }
}
