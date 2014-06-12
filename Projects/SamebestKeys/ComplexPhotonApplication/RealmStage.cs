using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    partial class RealmStage : Regulus.Game.IStage
    {
        private IRealm _Realm;        
        Player[] _Players;
        Regulus.Remoting.ISoulBinder _Binder;

        List<IObservedAbility> _Observeds;
        Action<IObservedAbility> _ObservedLeft;
        Action<IObservedAbility> _ObservedInto;
        Realm.Member _Member;
        public event Action ExitWorldEvent;
        public event Action LogoutEvent;        

        public RealmStage(Regulus.Remoting.ISoulBinder binder , IRealm realm ,Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation[] actors )
        {
            _Binder = binder;
            this._Realm = realm;
            
            _Players = (from actor in actors select new Player(actor)).ToArray();

            _Observeds = new List<IObservedAbility>();
        }

        void Game.IStage.Enter()
        {
            _Member = new Realm.Member(_Players[0]);
            _Member.BeginTraversableEvent += _OnBeginTraver;
            _Member.EndTraversableEvent += _OnEndTraver;
            if (_Realm.Join(_Member))
            {
                _Realm.ShutdownEvent += ExitWorldEvent;
                _Bind(_Players[0], _Binder);
                _RegisterQuit(_Players[0]);
            }
            else
                LogoutEvent();
        }

        private void _OnEndTraver(ITraversable traversable)
        {
            _Binder.Unbind<ITraversable>(traversable);
        }

        void _OnBeginTraver(ITraversable traversable)
        {
            _Binder.Bind<ITraversable>(traversable);
        }

        

        void Game.IStage.Leave()
        {
            _Member.BeginTraversableEvent -= _OnBeginTraver;
            _Member.EndTraversableEvent -= _OnEndTraver;

            _UnregisterQuit(_Players[0]);
            _Unbind(_Players[0], _Binder);
            _Realm.ShutdownEvent -= ExitWorldEvent;
            _Realm.Exit(_Players[0]);
        }

        private void _RegisterQuit(Player player)
        {
            player.ExitWorldEvent += ExitWorldEvent;            
            player.LogoutEvent += LogoutEvent;
        }

        private void _Bind(Player player, Remoting.ISoulBinder binder)
        {
            binder.Bind<IPlayer>(player);

            var observe = player.FindAbility<IObserveAbility>();
            if (observe != null)
            {
                _ObservedInto = (observed) =>
                {
                    binder.Bind<IObservedAbility>(observed);
                    _Observeds.Add(observed);
                };

                _ObservedLeft = (observed) =>
                {
                    _Observeds.Remove(observed);
                    binder.Unbind<IObservedAbility>(observed);
                };

                observe.IntoEvent += _ObservedInto;
                observe.LeftEvent += _ObservedLeft;
            }
            binder.Bind<Regulus.Remoting.ITime>(LocalTime.Instance);
        }

        private void _Unbind(Player player, Remoting.ISoulBinder binder)
        {
            binder.Unbind<Regulus.Remoting.ITime>(LocalTime.Instance);

            var observe = player.FindAbility<IObserveAbility>();
            if (observe != null)
            {
                observe.IntoEvent -= _ObservedInto;
                observe.LeftEvent -= _ObservedLeft;
            }
            foreach (var o in _Observeds)
            {
                binder.Unbind<IObservedAbility>(o);
            }

            binder.Unbind<IPlayer>(player);            
        }

        private void _UnregisterQuit(Player player)
        {
            player.ExitWorldEvent -= ExitWorldEvent;            
            player.LogoutEvent -= LogoutEvent;
        }

        void Game.IStage.Update()
        {            

        }
    }

    partial class Realm
    {
        class PlayingStage : Regulus.Game.IStage
        {
            private Zone _Zone;
            
            public delegate void OnDone();
            public event OnDone DoneEvent;

            Regulus.Utility.Updater _ZoneUpdater;
            public PlayingStage(Zone zone)
            {                
                this._Zone = zone;                
                _ZoneUpdater = new Utility.Updater();
            }
            void Game.IStage.Enter()
            {
                _ZoneUpdater.Add(_Zone);
            }

            void Game.IStage.Leave()
            {
                _ZoneUpdater.Shutdown();
            }

            void Game.IStage.Update()
            {
                _ZoneUpdater.Update();
            }
        }
    }

    partial class Realm
    {
        class ReadyStage : Regulus.Game.IStage
        {
            public delegate void OnDone();
            public event OnDone DoneEvent;
            private JoinCondition _JoinCondition;
            

            public ReadyStage()
            {                
                
            }

            public ReadyStage(JoinCondition join_condition)
            {                
                this._JoinCondition = join_condition;
            }

            void Game.IStage.Enter()
            {
                
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                if (_JoinCondition.LastCheck() == false)
                {
                    DoneEvent();
                }
            }
        }
    }
   
    
}
