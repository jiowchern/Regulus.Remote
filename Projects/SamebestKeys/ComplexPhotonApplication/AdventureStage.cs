using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class AdventureStage : Regulus.Game.IStage<User> 
    {
        IStorage _Stroage;
        DateTime _Save;
        Regulus.Project.SamebestKeys.Player _Player;
        List<IObservedAbility> _Observeds;
        IMap _Map;
        
		public AdventureStage(IMap map , IStorage storage)
		{
            _Observeds = new List<IObservedAbility>();
            _Stroage = storage;
            _Map = map;
		}
      
        Regulus.Game.StageLock Regulus.Game.IStage<User>.Enter(User obj)
        {            
            _Player = new Player(obj.Actor);
            obj.Provider.Bind<IPlayer>(_Player);
            _Player.Initial();
            _Player.ReadyEvent += _OnPlayerReady;            
            _Player.ExitWorldEvent += obj.ToParking;
            _Player.LogoutEvent += obj.Logout;
            _Player.CrossEvent += obj.OnCross;
			
            var observe = _Player.FindAbility<IObserveAbility>();
            if (observe != null)
            {
                _ObservedInto = (observed) =>
                {
                    obj.Provider.Bind<IObservedAbility>(observed);
                    _Observeds.Add(observed);
                };

                _ObservedLeft = (observed) =>
                {
                    _Observeds.Remove(observed);
                    obj.Provider.Unbind<IObservedAbility>(observed);
                };
                
                observe.IntoEvent += _ObservedInto;
                observe.LeftEvent += _ObservedLeft;
            }            
            obj.Provider.Bind<Regulus.Remoting.ITime>( LocalTime.Instance );

            _Save = DateTime.Now;

            _Map.Into(_Player);
            return null;
        }

        Action<IObservedAbility> _ObservedLeft;
        Action<IObservedAbility> _ObservedInto;
        

        void _OnPlayerReady()
        {
            _Player.ReadyEvent -= _OnPlayerReady;            
        }

        
        
        void Regulus.Game.IStage<User>.Leave(User obj)
        {            
            
            _Map.Left(_Player);
                

            obj.Provider.Unbind<Regulus.Remoting.ITime>(LocalTime.Instance);

            var observe = _Player.FindAbility<IObserveAbility>();
            if (observe != null)
            {
                observe.IntoEvent -= _ObservedInto;
                observe.LeftEvent -= _ObservedLeft;
            }
            foreach (var o in _Observeds)
            {
                obj.Provider.Unbind<IObservedAbility>(o);
            }
            _Player.Release();
            obj.Provider.Unbind<IPlayer>(_Player);
            
        }
      

        void Regulus.Game.IStage<User>.Update(User obj)
        {
            var elapsed = DateTime.Now.Ticks - _Save.Ticks;
            var span = new TimeSpan(elapsed);
            if (span.TotalMinutes > 1.0)
            {
                _Stroage.SaveActor(obj.Actor);                
                _Save = DateTime.Now;
            }
        }
    }
}
