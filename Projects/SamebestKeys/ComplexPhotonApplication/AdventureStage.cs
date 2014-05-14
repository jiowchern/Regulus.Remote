using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class AdventureStage : Regulus.Game.IStage
    {
        IStorage _Stroage;
        DateTime _Save;
        Regulus.Project.SamebestKeys.Player _Player;
        List<IObservedAbility> _Observeds;
        IMap _Map;
        User _User;
		public AdventureStage(IMap map , IStorage storage , User user)
		{
            _User = user;
            _Observeds = new List<IObservedAbility>();
            _Stroage = storage;
            _Map = map;
		}
      
        void Regulus.Game.IStage.Enter()
        {
            _Player = new Player(_User.Actor);
            _User.Provider.Bind<IPlayer>(_Player);
            _User.Provider.Bind<IMapInfomation>(_Map.GetInfomation());
            _Player.Initial();

            _Player.ExitWorldEvent += _User.ToParking;
            _Player.LogoutEvent += _User.Logout;
            _Player.CrossEvent += _User.ToCross;
			
            var observe = _Player.FindAbility<IObserveAbility>();
            if (observe != null)
            {
                _ObservedInto = (observed) =>
                {
                    _User.Provider.Bind<IObservedAbility>(observed);
                    _Observeds.Add(observed);
                };

                _ObservedLeft = (observed) =>
                {
                    _Observeds.Remove(observed);
                    _User.Provider.Unbind<IObservedAbility>(observed);
                };
                
                observe.IntoEvent += _ObservedInto;
                observe.LeftEvent += _ObservedLeft;
            }
            _User.Provider.Bind<Regulus.Remoting.ITime>(LocalTime.Instance);

            _Save = DateTime.Now;

            _Map.Into(_Player);
          
        }

        Action<IObservedAbility> _ObservedLeft;
        Action<IObservedAbility> _ObservedInto;
        
        
        void Regulus.Game.IStage.Leave()
        {            
            
            _Map.Left(_Player);


            _User.Provider.Unbind<Regulus.Remoting.ITime>(LocalTime.Instance);

            var observe = _Player.FindAbility<IObserveAbility>();
            if (observe != null)
            {
                observe.IntoEvent -= _ObservedInto;
                observe.LeftEvent -= _ObservedLeft;
            }
            foreach (var o in _Observeds)
            {
                _User.Provider.Unbind<IObservedAbility>(o);
            }
            _Player.Release();
            _User.Provider.Unbind<IPlayer>(_Player);
            _User.Provider.Unbind<IMapInfomation>(_Map.GetInfomation());
        }
      

        void Regulus.Game.IStage.Update()
        {
            var elapsed = DateTime.Now.Ticks - _Save.Ticks;
            var span = new TimeSpan(elapsed);
            if (span.TotalMinutes > 1.0)
            {
                _Stroage.SaveActor(_User.Actor);                
                _Save = DateTime.Now;
            }
        }
    }
}
