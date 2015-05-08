using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    class ConnectStage : Regulus.Utility.IStage
    {
        private Remoting.Ghost.IProviderNotice<IConnect> _Provider;
        private string _Ip;
        private int _Port;

        public delegate void DoneCallback();
        public event DoneCallback SuccessEvent;
        public event DoneCallback FailEvent;

        public ConnectStage(Remoting.Ghost.IProviderNotice<IConnect> provider, string ip, int port)
        {
            
            this._Provider = provider;
            this._Ip = ip;
            this._Port = port;
        }

        void IStage.Enter()
        {
            _Provider.Supply += _Provider_Supply;
        }

        void _Provider_Supply(IConnect obj)
        {
            obj.Connect(_Ip, _Port).OnValue += _Result;
        }

        private void _Result(bool success)
        {
            if (success)
                SuccessEvent();
            else
                FailEvent();
        }

        void IStage.Leave()
        {
            _Provider.Supply -= _Provider_Supply;
        }

        void IStage.Update()
        {
            
        }
    }
}
