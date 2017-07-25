using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Regulus.Network.RUDP;
using Regulus.Network.Win32;
using Regulus.Utility;

namespace Regulus.Network
{
    public class RudpListener : ISocketLintenable
    {
        private readonly ITime _Time;
        private Host _Host;
        private bool _Enable;
        private event Action<ISocket> _AcctpeEvent;

        public RudpListener(ITime time)
        {
            _Time = time;
            Timestamp.OneSecondTicks = _Time.OneSeconds;
        }

        event Action<ISocket> ISocketLintenable.AcceptEvent
        {
            add { _AcctpeEvent += value; }
            remove { _AcctpeEvent -= value; }
        }

        void ISocketLintenable.Bind(int port)
        {
            _Host = Host.CreateStandard(port);
            _Host.AcceptEvent += _Accept;
            _Enable = true;
            ThreadPool.QueueUserWorkItem(_Run, null);
        }

        private void _Run(object state)
        {
            var updater = new Regulus.Utility.Updater<Timestamp>();
            updater.Add(_Host);
            
            var regulator = new Regulus.Utility.AutoPowerRegulator(new PowerRegulator());
            while (_Enable)
            {
                _Time.Sample();
                updater.Working(new Timestamp(_Time.Now, _Time.Delta));
                regulator.Operate();
            }

            updater.Shutdown();

        }

        private void _Accept(IPeer peer)
        {
            _AcctpeEvent(new RudpSocket(peer));
        }

        void ISocketLintenable.Close()
        {
            _Host.AcceptEvent -= _Accept;
            _Enable = false;
        }
    }
}
