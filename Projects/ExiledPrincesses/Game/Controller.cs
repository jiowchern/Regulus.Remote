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
        
    }
    
    public class PlayerController : Controller
    {
        public class Binder<T>   where T :class
        {
            private Remoting.ISoulBinder _Binder;
            T _Object;
            public Binder(Remoting.ISoulBinder binder)
            {
                _Binder = binder;
            }
            public void Set(T obj)
            {
                if (_Object != default(T))
                {
                    _Binder.Unbind<T>(_Object);
                }
                if(obj != default(T))
                    _Binder.Bind<T>(obj);
                _Object = obj;
            }
        }

        private Remoting.ISoulBinder _Binder;

        Binder<IAdventureIdle> _AdventureIdleBinder;
        Binder<IAdventureGo> _AdventureGoBinder;
        Binder<IAdventureChoice> _AdventureChoiceBinder;       
        public PlayerController(Remoting.ISoulBinder binder)  
        {           
            this._Binder = binder;

            _AdventureIdleBinder = new Binder<IAdventureIdle>(_Binder);
            SetIdleController += (obj) =>
            {
                _AdventureIdleBinder.Set(obj);
            };

            _AdventureGoBinder = new Binder<IAdventureGo>(_Binder);
            SetGoController += (obj) =>
            {
                _AdventureGoBinder.Set(obj);
            };

            _AdventureChoiceBinder = new Binder<IAdventureChoice>(_Binder);
            SetChoiceController += (obj) =>
            {
                _AdventureChoiceBinder.Set(obj);
            };
        }
        
    }
}
