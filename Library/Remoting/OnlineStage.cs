using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting
{
    class OnlineStage : Regulus.Game.IStage
    {
        private Regulus.Remoting.IAgent _Agent;
        private Regulus.Remoting.Ghost.TProvider<Regulus.Game.IOnline> _OnlineProvider;
        Regulus.Game.Online _Online;

        public event Action BreakEvent
        {
            add { _Agent.DisconnectEvent += value; }
            remove { _Agent.DisconnectEvent -= value; }
        }

        public OnlineStage(Regulus.Remoting.IAgent agent, Regulus.Remoting.Ghost.TProvider<Regulus.Game.IOnline> provider)
        {            
            this._Agent = agent;
            this._OnlineProvider = provider;
            _Online = new Regulus.Game.Online(agent);
            
        }
        void Regulus.Game.IStage.Enter()
        {
            _Bind(_OnlineProvider);
        }

        private void _Bind(Regulus.Remoting.Ghost.IProvider provider)
        {
            provider.Add(_Online);
            provider.Ready(_Online.Id);
        }
        private void _Unbind(Regulus.Remoting.Ghost.IProvider provider)
        {
            provider.Remove(_Online.Id);            
        }

        void Regulus.Game.IStage.Leave()
        {
            _Unbind(_OnlineProvider);
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
