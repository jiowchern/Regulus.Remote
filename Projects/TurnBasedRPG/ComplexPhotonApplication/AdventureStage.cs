using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class AdventureStage : Regulus.Game.IStage<User>
    {
        DateTime _Save;
        Regulus.Project.TurnBasedRPG.Player _Player;
        IWorld _World;

		public AdventureStage(IWorld world)
		{
			_World = world;
		}
      
        Regulus.Game.StageLock Regulus.Game.IStage<User>.Enter(User obj)
        {            
            _Player = new Player(obj.Actor);
            obj.Provider.Bind<IPlayer>(_Player);
            _Player.Initial();
            _Player.ReadyEvent += _OnPlayerReady;            
            _Player.ExitWorldEvent += obj.ToParking;
            _Player.LogoutEvent += obj.Logout;
			
            var observe = _Player.FindAbility<IObserveAbility>();
            if (observe != null)
            {
                _ObservedInto = (observed) =>
                {
                    obj.Provider.Bind<IObservedAbility>(observed);
                };

                _ObservedLeft = (observed) =>
                {
                    obj.Provider.Unbind<IObservedAbility>(observed);
                };
                
                observe.IntoEvent += _ObservedInto;
                observe.LeftEvent += _ObservedLeft;
            }            
            obj.Provider.Bind<Regulus.Remoting.ITime>( LocalTime.Instance );
            
            _Save = DateTime.Now;

            
            return null;
        }

        Action<IObservedAbility> _ObservedLeft;
        Action<IObservedAbility> _ObservedInto;
        

        void _OnPlayerReady()
        {
            _Player.ReadyEvent -= _OnPlayerReady;
            var mapValue = _World.Find(_Player.Map);
            mapValue.OnValue += _IntoMap;			
        }

        void _IntoMap(IMap map)
        {
            if (map != null)
            {
                map.Into(_Player);
            }
        }
        
        void Regulus.Game.IStage<User>.Leave(User obj)
        {
            var mapValue = _World.Find(_Player.Map);
            mapValue.OnValue += _LeftMap;

            if (_Player != null)
            {
                obj.Provider.Unbind<Regulus.Remoting.ITime>(LocalTime.Instance);

                var observe = _Player.FindAbility<IObserveAbility>();
                if (observe != null)
                {
                    observe.IntoEvent -= _ObservedInto;
                    observe.LeftEvent -= _ObservedLeft;
                }
                _Player.Release();
                obj.Provider.Unbind<IPlayer>(_Player);
            }
        }

        void _LeftMap(IMap map)
        {
            if (map != null)
                map.Left(_Player);
        }

        void Regulus.Game.IStage<User>.Update(User obj)
        {
            var elapsed = DateTime.Now.Ticks - _Save.Ticks;
            var span = new TimeSpan(elapsed);
            if (span.TotalMinutes > 1.0)
            {
                Regulus.Utility.Singleton<Storage>.Instance.SaveActor(obj.Actor);                
                _Save = DateTime.Now;
            }
        }
    }
}
