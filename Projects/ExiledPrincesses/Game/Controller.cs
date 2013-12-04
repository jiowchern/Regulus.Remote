using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public class Controller
    {
        public delegate void OnIdleController(IAdventureIdle adventure_idle);
        public OnIdleController SetIdleController;

        public delegate void OnGoController(IAdventureGo adventure_go);
        public OnGoController SetGoController;

        public delegate void OnChoiceController(IAdventureChoice adventure_choice);
        public OnChoiceController SetChoiceController;

        public delegate void OnActors(IActor[] actors);
        public OnActors SetComrades;
        public OnActors SetEnemys;

        public delegate void OnTeamss(ITeam[] teams);
        public OnTeamss SetTeams;

        public delegate void OnCombatController(ITeammate[] teammates);
        public OnCombatController SetCombatController;
        public Controller()
        {
            SetIdleController = (p) => { };
            SetGoController = (p) => { };
            SetChoiceController = (p) => { };
            SetComrades = (p) => { };
            SetEnemys = (p) => { };
            SetTeams = (p) => { };
            SetCombatController = (p) => { };
        }
    }
    
    public class PlayerController : Controller
    {
        public class Binder<T> where T : class
        {
            private Remoting.ISoulBinder _Binder;
            T[] _Objects;
            public Binder(Remoting.ISoulBinder binder , T[] objs)
            {
                _Binder = binder;
                _Objects = objs;
            }

            public void Differences(T[] source)
            {
                _Differences(_Objects, source);
                _Objects = source;
            }
            private void _Differences(T[] current, T[] actors)
            {
                var exits = current.Except(actors);
                foreach (var exit in exits)
                {
                    _Binder.Unbind<T>(exit);
                }

                var joins = actors.Except(current);
                foreach (var join in joins)
                {
                    _Binder.Bind<T>(join);
                }

            }
        }
        public class OnesBinder<T>   where T :class
        {
            private Remoting.ISoulBinder _Binder;
            T _Object;
            public OnesBinder(Remoting.ISoulBinder binder)
            {
                _Binder = binder;
            }
            public void Set(T obj)
            {
                if (_Object != default(T) )
                {
                    _Binder.Unbind<T>(_Object);
                    if(obj != default(T))
                        throw new SystemException("OnesBinder 重複綁定");                    
                }
                if(obj != default(T))
                    _Binder.Bind<T>(obj);
                _Object = obj;
            }
        }

        private Remoting.ISoulBinder _Binder;

        OnesBinder<IAdventureIdle> _AdventureIdleBinder;
        OnesBinder<IAdventureGo> _AdventureGoBinder;
        OnesBinder<IAdventureChoice> _AdventureChoiceBinder;       
        public PlayerController(Remoting.ISoulBinder binder)  
        {
            
            this._Binder = binder;

            _AdventureIdleBinder = new OnesBinder<IAdventureIdle>(_Binder);
            SetIdleController += (obj) =>
            {
                _AdventureIdleBinder.Set(obj);
            };

            _AdventureGoBinder = new OnesBinder<IAdventureGo>(_Binder);
            SetGoController += (obj) =>
            {
                _AdventureGoBinder.Set(obj);
            };

            _AdventureChoiceBinder = new OnesBinder<IAdventureChoice>(_Binder);
            SetChoiceController += (obj) =>
            {
                _AdventureChoiceBinder.Set(obj);
            };

            _Comrade = new Binder<IActor>(_Binder , new IActor[0]);
            SetComrades += _OnComrades;

            _Enemy = new Binder<IActor>(_Binder, new IActor[0]);
            SetEnemys += _OnEnemys;

            _Team = new Binder<ITeam>(_Binder, new ITeam[0]);
            SetTeams += _OnTeams;

            _CombatController = new Binder<ICombatController>(_Binder ,new ICombatController[0] );
            SetCombatController += _OnCombatController;
        }

        private void _OnCombatController(ITeammate[] teammates)
        {
            _CombatController.Differences(teammates);
        }

        private void _OnTeams(ITeam[] teams)
        {
            _Team.Differences(teams);
        }
        Binder<IActor> _Comrade;        
        private void _OnComrades(IActor[] actors)
        {
            _Comrade.Differences(actors);
        }

        Binder<IActor> _Enemy;
        private Binder<ICombatController> _CombatController;                
        private void _OnEnemys(IActor[] actors)
        {
            _Enemy.Differences(actors);
        }

        public Binder<ITeam> _Team { get; set; }
    }
}
