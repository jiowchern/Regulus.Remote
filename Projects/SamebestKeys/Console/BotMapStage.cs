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
            _Timeup = Regulus.Utility.Random.Next(10, 60 * 10);
        }
        public void Enter()
        {
            _User.TraversableProvider.Supply += TraversableProvider_Supply;
            _User.OnlineProvider.Supply += OnlineProvider_Supply;
            _User.PlayerProvider.Supply += PlayerProvider_Supply;
            
        }

        void TraversableProvider_Supply(Regulus.Project.SamebestKeys.ITraversable obj)
        {
            obj.Ready();            
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
            if (obj.ActionStatue == Regulus.Project.SamebestKeys.ActionStatue.Idle && obj.Speed == 0)
            {
                var angle = Regulus.Utility.Random.Next(0, 360);
                _Player.Walk(angle);
                //_Player.Say("撞到! 轉向" + angle + "度移動");
            }
        }

        void PlayerProvider_Supply(Regulus.Project.SamebestKeys.IPlayer obj)
        {
            _Player = obj;
            //_Player.SetPosition(Regulus.Utility.Random.Next(0 , 100), Regulus.Utility.Random.Next(0, 100));
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
            _User.TraversableProvider.Supply -= TraversableProvider_Supply;
        }

        public void Update()
        {

            if (_TimeCounter.Second > _Timeup)
            {
                if (_Player != null)                
                {
                    _Player.Stop(0);
                    var map = Regulus.Utility.Random.Next(0, 3);
                    if (map == 0)
                    {
                        _Player.Goto("Ark", Regulus.Utility.Random.Next(3, 3 + 10), Regulus.Utility.Random.Next(389, 389+10));
                    }
                    if (map == 1)
                        _Player.Goto("Test", Regulus.Utility.Random.Next(50, 50 + 10), Regulus.Utility.Random.Next(50, 50 + 10));
                    if (map == 2)
                        _Player.Goto("SL_1C", Regulus.Utility.Random.Next(200, 200 + 10), Regulus.Utility.Random.Next(200, 200 + 10));

                    
                }
                    
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
