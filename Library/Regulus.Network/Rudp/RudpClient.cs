using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Regulus.Network.RUDP;

namespace Regulus.Network.Rudp
{
    public class RudpClient : ISocketClient
    {
        private readonly ITime _Time;
        private readonly Agent _Agent;
        private bool _Enable;

        public RudpClient()
        {
            _Time = Timestamp.Time;
            _Agent = Agent.CreateStandard();
            
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


        void ISocketClient.Launch()
        {
            _Enable = true;
            ThreadPool.QueueUserWorkItem(_Run, null);
        }

        void ISocketClient.Shutdown()
        {
            _Enable = false;
        }

        ISocketConnectable ISocketClient.Spawn()
        {
            return new RudpConnecter(_Agent);
        }
    }
}
