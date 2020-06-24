using Regulus.Utility;
using System;
using System.Net;

namespace Regulus.Remote.Standalone
{
    internal class OfflineStatus : IStatus
    {
        private IProvider _ConnectProvider;
        private ConnectGhost _Ghost;

        public event System.Action DoneEvent;
        public OfflineStatus(IProvider connectProvider)
        {
            _Ghost = new ConnectGhost();
            _Ghost.ConnectedEvent += _Done;
            _ConnectProvider = connectProvider;
        }

        private void _Done(IPEndPoint arg1, Value<bool> arg2)
        {
            arg2.SetValue(true);
            DoneEvent();
        }

        void IStatus.Enter()
        {
            _ConnectProvider.Add(_Ghost);
            _ConnectProvider.Ready(_Ghost.Id);
        }

        void IStatus.Leave()
        {
            _ConnectProvider.Remove(_Ghost.Id);
        }

        void IStatus.Update()
        {
        }
    }
}