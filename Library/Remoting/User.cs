using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    public class User : Regulus.Utility.IUpdatable
    {
        Regulus.Remoting.Ghost.TProvider<Regulus.Game.IConnect> _ConnectProvider;
        Regulus.Remoting.Ghost.TProvider<Regulus.Game.IOnline> _OnlineProvider;        
        Regulus.Game.StageMachine _Machine;
        Regulus.Remoting.IAgent _Agent;

        Regulus.Utility.Updater _Updater;

        public User(Regulus.Remoting.IAgent agent)
        {
            _Agent = agent;
            _ConnectProvider = new Regulus.Remoting.Ghost.TProvider<Regulus.Game.IConnect>();
            _OnlineProvider = new Regulus.Remoting.Ghost.TProvider<Regulus.Game.IOnline>();
            _Machine = new Regulus.Game.StageMachine();
            _Updater = new Regulus.Utility.Updater();
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Update();
            _Machine.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Updater.Add(_Agent);
            _ToOffline();
        }

        private void _ToOffline()
        {
            var stage = new OfflineStage(_Agent , _ConnectProvider);

            stage.DoneEvent += _ToOnline;

            _Machine.Push(stage);
        }

        private void _ToOnline()
        {
            var stage = new OnlineStage(_Agent, _OnlineProvider);

            stage.BreakEvent += _ToOffline;
            
            _Machine.Push(stage);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
            _Updater.Shutdown();
        }

        public Regulus.Remoting.Ghost.IProviderNotice<Regulus.Game.IConnect> ConnectProvider
        {
            get { return _ConnectProvider; }
        }

        public Regulus.Remoting.Ghost.IProviderNotice<Regulus.Game.IOnline> OnlineProvider
        {
            get { return _OnlineProvider; }
        }
    }
}
