using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
    public class User : Regulus.Utility.IUpdatable
    {
        Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IConnect> _ConnectProvider;
        Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IOnline> _OnlineProvider;        
        Regulus.Utility.StageMachine _Machine;
        Regulus.Remoting.IAgent _Agent;

        Regulus.Utility.Updater _Updater;

        public User(Regulus.Remoting.IAgent agent)
        {
            _Agent = agent;
            _ConnectProvider = new Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IConnect>();
            _OnlineProvider = new Regulus.Remoting.Ghost.TProvider<Regulus.Utility.IOnline>();
            _Machine = new Regulus.Utility.StageMachine();
            _Updater = new Regulus.Utility.Updater();
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            _Machine.Update();
            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Updater.Add(_Agent);
            _ToOffline();
        }

        private void _ToOffline()
        {
            var stage = new OfflineStage(_Agent , _ConnectProvider);

            stage.DoneEvent += _ToOnline;
            Regulus.Utility.Log.Instance.Write("1.user start offline");
            _Machine.Push(stage);
        }

        private void _ToOnline()
        {
            var stage = new OnlineStage(_Agent, _OnlineProvider);

            
            stage.BreakEvent += _ToOffline;
            Regulus.Utility.Log.Instance.Write("5.user start online");
            _Machine.Push(stage);
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Machine.Termination();
            _Updater.Shutdown();
        }

        public Regulus.Remoting.Ghost.INotifier<Regulus.Utility.IConnect> ConnectProvider
        {
            get { return _ConnectProvider; }
        }

        public Regulus.Remoting.Ghost.INotifier<Regulus.Utility.IOnline> OnlineProvider
        {
            get { return _OnlineProvider; }
        }
    }
}
