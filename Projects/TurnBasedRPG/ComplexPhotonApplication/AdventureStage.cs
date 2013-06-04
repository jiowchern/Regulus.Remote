using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class AdventureStage : Samebest.Game.IStage<User>
    {
        DateTime _Save;
        Regulus.Project.TurnBasedRPG.Player _Player;
        
        void Samebest.Game.IStage<User>.Enter(User obj)
        {            
            _Player = new Player(obj.Actor);
            obj.Provider.Bind<IPlayer>(_Player);
            _Player.Initialize();
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
            
            _Save = DateTime.Now;
        }

        Action<IObservedAbility> _ObservedLeft;
        Action<IObservedAbility> _ObservedInto;
        

        void _OnPlayerReady()
        {
            _Player.ReadyEvent -= _OnPlayerReady;
            Samebest.Utility.Singleton<Map>.Instance.Into(_Player, _ExitMap);            
			
        }

        void _ExitMap()
        {
            var plr = _Player as IPlayer;
            // 離開地圖
            // 可能是切換地圖
        }

        void Samebest.Game.IStage<User>.Leave(User obj)
        {
            if (_Player != null)
            {
                var observe = _Player.FindAbility<IObserveAbility>();
                if (observe != null)
                {
                    observe.IntoEvent -= _ObservedInto;
                    observe.LeftEvent -= _ObservedLeft;
                }                
                Samebest.Utility.Singleton<Map>.Instance.Left(_Player);
                _Player.Finialize();
                obj.Provider.Unbind<IPlayer>(_Player);
            }
            
        }

        void Samebest.Game.IStage<User>.Update(User obj)
        {
            var elapsed = DateTime.Now.Ticks - _Save.Ticks;
            var span = new TimeSpan(elapsed);
            if (span.TotalMinutes > 1.0)
            {
                Samebest.Utility.Singleton<Storage>.Instance.SaveActor(obj.Actor);
                _Save = DateTime.Now;
            }
        }
    }
}
