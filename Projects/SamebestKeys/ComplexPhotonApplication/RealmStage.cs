using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Dungeons
{
    partial class RealmStage : Regulus.Utility.IStage, IRealmJumper
    {
        Belongings _Belongings;
        Regulus.Utility.Updater _Updater;
        private IScene _Scene;        
        Player[] _Players;
        Player _Player {get {return _Players[0];}}
        Regulus.Remoting.ISoulBinder _Binder;

        List<IObservedAbility> _Observeds;
        Action<IObservedAbility> _ObservedLeft;
        Action<IObservedAbility> _ObservedInto;
        Member _Member;
        public event Action ExitWorldEvent;
        public event Action LogoutEvent;
        public event Action<string> ChangeRealmEvent;
        public event Action QuitEvent;

        ITraversable _Traversable;
        
        public RealmStage(Regulus.Remoting.ISoulBinder binder, IScene realm, Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation[] actors, Belongings belongings)
        {
            
            _Binder = binder;
            this._Scene = realm;
            
            _Players = (from actor in actors select new Player(actor)).ToArray();

            _Observeds = new List<IObservedAbility>();
            _Belongings = belongings;
            _Updater = new Utility.Updater();
         
        }

        void Utility.IStage.Enter() 
        {
            _Binder.Bind<IBelongings>(_Belongings);
            _Member = new Member(_Player);
            _Member.BeginTraversable+= _OnBeginTraver;
            _Member.EndTraversable+= _OnEndTraver;
            _Scene.ShutdownEvent += ExitWorldEvent;

            _RegisterQuit(_Player);

            if (_Scene.Join(_Member) == false)
            {
                LogoutEvent();
            }


            _Updater.Add(_Belongings);

            _InitialSession();
        }

        private void _InitialSession()
        {
            var student = _Player.FindAbility<Session.StuffStudent>();
            if (student != null)
            {
                student.SetBinder(_Binder);
            }

            var teacher = _Player.FindAbility<Session.StuffTeacher>();
            if (teacher != null)
            {
                teacher.SetBinder(_Binder);
            }
        }

        void _ResleaseSession()
        {
            var student = _Player.FindAbility<Session.StuffStudent>();
            if (student != null)
            {
                student.ClearBinder();
            }

            var teacher = _Player.FindAbility<Session.StuffTeacher>();
            if (teacher != null)
            {
                teacher.ClearBinder();
            }
        }

        private void _OnEndTraver(ITraversable traversable)
        {
            _Traversable = null;
            _Binder.Unbind<ITraversable>(traversable);
            _Bind(_Player, _Binder);
        }

        void _OnBeginTraver(ITraversable traversable)
        {
            _Traversable = traversable;
            _Unbind(_Player, _Binder);            
            _Binder.Bind<ITraversable>(traversable);
        }
        void Utility.IStage.Leave()
        {
            _ResleaseSession();
            
            if (_Traversable != null)
            {
                _Binder.Unbind<ITraversable>(_Traversable); 
                _Traversable = null;
            }

            _Member.BeginTraversable -= _OnBeginTraver;
            _Member.EndTraversable -= _OnEndTraver;
            
            _UnregisterQuit(_Player);
            _Unbind(_Player, _Binder);            
            _Scene.ShutdownEvent -= ExitWorldEvent;
            _Scene.Exit(_Member);
            _Binder.Unbind<IBelongings>(_Belongings);

            _Updater.Shutdown();
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

            binder.Bind<IRealmJumper>(this);
        }

        private void _Unbind(Player player, Remoting.ISoulBinder binder)
        {
            binder.Unbind<IRealmJumper>(this);

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

        void Utility.IStage.Update()
        {
            _Updater.Update();
        }

        void IRealmJumper.Jump(string realm)
        {
            ChangeRealmEvent(realm);            
        }

        Remoting.Value<string[]> IRealmJumper.Query()
        {
            return _QueryScenes(_Player);
        }

        private Remoting.Value<string[]> _QueryScenes(Player player)
        {
            return player.QueryPlayableScenes();
        }




        void IRealmJumper.Quit()
        {
            QuitEvent();
        }
    }

    
}
