using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotMapStage : Regulus.Game.IStage
    {
        private Regulus.Project.SamebestKeys.IUser _User;
        Regulus.Project.SamebestKeys.IOnline _Online;
        Regulus.Utility.TimeCounter _TimeCounter;
        Regulus.Project.SamebestKeys.IPlayer _Player;
        public enum Result
        {
            Connect,Reset
        };
        public event Action<Result> ResultEvent;
        public BotMapStage(Regulus.Project.SamebestKeys.IUser _User)
        {            
            this._User = _User;
            _TimeCounter = new Regulus.Utility.TimeCounter();
            _Timeup = Regulus.Utility.Random.Next(5, 15);
        }
        public void Enter()
        {
            _User.OnlineProvider.Supply += OnlineProvider_Supply;
            _User.PlayerProvider.Supply += PlayerProvider_Supply;
            
        }

        void ObservedAbilityProvider_Supply(Regulus.Project.SamebestKeys.IObservedAbility obj)
        {
            if (obj.Id == _Player.Id)
            {
                obj.ShowActionEvent += obj_ShowActionEvent;
            }
        }

        void obj_ShowActionEvent(Regulus.Project.SamebestKeys.Serializable.MoveInfomation obj)
        {
            if (obj.ActionStatue == Regulus.Project.SamebestKeys.ActionStatue.GangnamStyle)
            {
                _Player.Walk(Regulus.Utility.Random.Next(0, 360));
            }
        }

        void PlayerProvider_Supply(Regulus.Project.SamebestKeys.IPlayer obj)
        {
            _Player = obj;
            _Player.SetPosition(Regulus.Utility.Random.Next(0 , 100), Regulus.Utility.Random.Next(0, 100));
            _Player.SetSpeed(5);
            _Player.Walk(Regulus.Utility.Random.Next(0,360));
            _TimeCounter.Reset();

            _User.ObservedAbilityProvider.Supply += ObservedAbilityProvider_Supply;
        }

        void OnlineProvider_Supply(Regulus.Project.SamebestKeys.IOnline obj)
        {
            _Online = obj;
        }

        public void Leave()
        {
            _User.ObservedAbilityProvider.Supply -= ObservedAbilityProvider_Supply;
            _User.PlayerProvider.Supply -= PlayerProvider_Supply;
            _User.OnlineProvider.Supply -= OnlineProvider_Supply;            
        }

        public void Update()
        {

            if (_TimeCounter.Second > _Timeup)
            {
                if (_Player != null)
                    _Player.Stop(0);
                if (_Online != null)
                {
                    _Online.Disconnect();
                    ResultEvent(Regulus.Utility.Random.Next(0, 10) > 5 ? Result.Connect : Result.Reset);
                }
            }
            
        }

        int _Timeup { get; set; }
    }
}
