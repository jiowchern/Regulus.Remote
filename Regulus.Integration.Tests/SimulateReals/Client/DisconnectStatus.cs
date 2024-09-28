using Regulus.Network.Tcp;
using Regulus.Utility;

namespace Regulus.Integration.Tests.SimulateReals.Client
{
    internal class DisconnectStatus :Regulus.Utility.IStatus
    {
        private Connector _Connector;
        bool _Done;
        public event System.Action DoneEvent;
        public DisconnectStatus(Connector connector)
        {
            this._Connector = connector;
        }

        async void IStatus.Enter()
        {
            await _Connector.Disconnect();
            _Done = true;
        }

        void IStatus.Leave()
        {
            
        }

        void IStatus.Update()
        {
            if(_Done)
            {
                DoneEvent();
            }
        }
    }
}