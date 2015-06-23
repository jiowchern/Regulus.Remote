using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting
{
    class OnlineStage : Regulus.Utility.IStage
    {
        private Regulus.Remoting.IAgent _Agent;
        private Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IOnline> _OnlineProvider;
        Regulus.Utility.Online _Online;


        event Action _BreakEvent;
        public event Action BreakEvent
        {
            add { _BreakEvent += value; }
            remove { _BreakEvent -= value; }
        }

        public OnlineStage(Regulus.Remoting.IAgent agent, Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IOnline> provider)
        {            
            this._Agent = agent;
            this._OnlineProvider = provider;
            _Online = new Regulus.Utility.Online(agent);
            
        }
        void Regulus.Utility.IStage.Enter()
        {
            _Agent.BreakEvent += _BreakEvent;            
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

        void Regulus.Utility.IStage.Leave()
        {
            _Unbind(_OnlineProvider);
            _Agent.BreakEvent -= _BreakEvent;            
            
        }

        void Regulus.Utility.IStage.Update()
        {
            if (_Agent.Connected == false)
                _BreakEvent();
        }
    }
}
