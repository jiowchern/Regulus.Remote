using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{
    class OfflineStage : Regulus.Game.IStage
    {
        private Regulus.Remoting.IAgent _Agent;
        private Regulus.Remoting.Ghost.TProvider<Regulus.Game.IConnect> _ConnectProvider;
        Regulus.Game.Connect _Connect;


        public delegate void OnDone();
        public event OnDone DoneEvent;

        public OfflineStage(Regulus.Remoting.IAgent agent, Regulus.Remoting.Ghost.TProvider<Regulus.Game.IConnect> _ConnectProvider)
        {            
            this._Agent = agent;
            this._ConnectProvider = _ConnectProvider;
            _Connect = new Regulus.Game.Connect();
        }
        void Regulus.Game.IStage.Enter()
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

        void Regulus.Game.IStage.Leave()
        {
            _Unbind(_ConnectProvider);

            _Connect.ConnectedEvent -= _Connect_ConnectedEvent;
        }

        void Regulus.Game.IStage.Update()
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
