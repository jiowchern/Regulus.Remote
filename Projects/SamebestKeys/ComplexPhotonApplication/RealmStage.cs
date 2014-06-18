using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Dungeons
{
    partial class RealmStage : Regulus.Game.IStage
    {
        private IScene _Realm;        
        Player[] _Players;
        Regulus.Remoting.ISoulBinder _Binder;

        List<IObservedAbility> _Observeds;
        Action<IObservedAbility> _ObservedLeft;
        Action<IObservedAbility> _ObservedInto;
        Member _Member;
        public event Action ExitWorldEvent;
        public event Action LogoutEvent;
        public event Action<string> ChangeRealmEvent;

        public RealmStage(Regulus.Remoting.ISoulBinder binder, IScene realm, Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation[] actors)
        {
            _Binder = binder;
            this._Realm = realm;
            
            _Players = (from actor in actors select new Player(actor)).ToArray();

            _Observeds = new List<IObservedAbility>();
        }

        void Game.IStage.Enter() 
        {
            _Member = new Member(_Players[0]);
            _Member.BeginTraversableEvent += _OnBeginTraver;
            _Member.EndTraversableEvent += _OnEndTraver;
            _Realm.ShutdownEvent += ExitWorldEvent;
            _Bind(_Players[0], _Binder);
            _RegisterQuit(_Players[0]);

            if (_Realm.Join(_Member)== false)
            {
                LogoutEvent();
            }            
                
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

    
}
