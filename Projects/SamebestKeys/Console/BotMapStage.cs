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
                var angle = Regulus.Utility.Random.Next(0, 360);
                _Player.Walk(angle);
                _Player.Say("撞到! 轉向" + angle + "度移動");
            }
        }

        void PlayerProvider_Supply(Regulus.Project.SamebestKeys.IPlayer obj)
        {
            _Player = obj;
            _Player.SetPosition(Regulus.Utility.Random.Next(0 , 100), Regulus.Utility.Random.Next(0, 100));
            _Player.SetSpeed(5);
            _Player.SetVision(30);
            var angle = Regulus.Utility.Random.Next(0, 360);
            _Player.Walk(angle);
            //_Player.Say("轉向"+angle+"度移動"  );
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
                    
                    var result = Regulus.Utility.Random.Next(0, 10) >= 9 ? Result.Connect : Result.Reset;
                    if (result == Result.Connect)
                    {
                        _Online.Disconnect();
                    }
                    ResultEvent(result);
                }
            }
            
        }

        int _Timeup { get; set; }
    }
}
