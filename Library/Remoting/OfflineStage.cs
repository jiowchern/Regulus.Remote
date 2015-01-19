using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    class OfflineStage : Regulus.Utility.IStage
    {
        private Regulus.Remoting.IAgent _Agent;
        private Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IConnect> _ConnectProvider;
        Regulus.Utility.Connect _Connect;


        public delegate void OnDone();
        public event OnDone DoneEvent;

        public OfflineStage(Regulus.Remoting.IAgent agent, Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IConnect> _ConnectProvider)
        {            
            this._Agent = agent;
            this._ConnectProvider = _ConnectProvider;
            _Connect = new Regulus.Utility.Connect();
        }
        void Regulus.Utility.IStage.Enter()
        {
            _Connect.ConnectedEvent += _Connect_ConnectedEvent;

            _Bind(_ConnectProvider);
        }

        void _Connect_ConnectedEvent(string account, int password, Regulus.Remoting.Value<bool> result)
        {
            Regulus.Remoting.Value<bool> r = _Agent.Connect(account, password);
            r.OnValue += (success) =>
            {
                DoneEvent();
                result.SetValue(success);
            };
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Unbind(_ConnectProvider);

            _Connect.ConnectedEvent -= _Connect_ConnectedEvent;
        }

        void Regulus.Utility.IStage.Update()
        {            
        }

        void _Bind(Regulus.Remoting.Ghost.IProvider provider)
        {
            provider.Add(_Connect);
            provider.Ready(_Connect.Id);
        }

        void _Unbind(Regulus.Remoting.Ghost.IProvider provider)
        {
            provider.Remove(_Connect.Id);
        }
    }
}
