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
        
    }
    
    public class PlayerController : Controller
    {
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
            _Actors = new IActor[0];
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

            SetComrades += _OnComrades;
        }
        IActor[] _Actors;
        private void _OnComrades(IActor[] actors)
        {
            var exits = _Actors.Except(actors);
            foreach(var exit in exits)
            {
                _Binder.Unbind<IActor>(exit);
            }

            var joins = actors.Except(_Actors);
            foreach (var join in joins)
            {
                _Binder.Bind<IActor>(join);
            }
            _Actors = actors;
        }
        
    }
}
