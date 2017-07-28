using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Regulus.Network.RUDP;

namespace Regulus.Network.Rudp
{
    public class RudpClient : IPeerClient
    {
        private readonly ISocket _Socket;
        private readonly ITime _Time;
        private readonly Agent _Agent;
        private bool _Enable;

        public RudpClient(ISocket socket)
        {
            _Socket = socket;
            _Time = new Time();
            _Agent = new Agent(_Socket, _Socket);
            
        }

        private void _Run(object state)
        {
            var updater = new Regulus.Utility.Updater<Timestamp>();
            updater.Add(_Agent);
            while (_Enable)
            {
                _Time.Sample();
                updater.Working(new Timestamp(_Time.Now , _Time.Delta));
            }
            updater.Shutdown();
        }


        void IPeerClient.Launch()
        {
            _Socket.Bind(0);
            _Enable = true;
            ThreadPool.QueueUserWorkItem(_Run, null);
        }

        void IPeerClient.Shutdown()
        {
            _Enable = false;
        }

        IPeerConnectable IPeerClient.Spawn()
        {
            return new RudpConnecter(_Agent);
        }
    }
}
