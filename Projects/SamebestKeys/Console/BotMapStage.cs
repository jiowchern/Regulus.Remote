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
        public event Action ResultEvent;
        public BotMapStage(Regulus.Project.SamebestKeys.IUser _User)
        {            
            this._User = _User;
            _TimeCounter = new Regulus.Utility.TimeCounter();
        }
        public void Enter()
        {
            _User.OnlineProvider.Supply += OnlineProvider_Supply;
            _User.PlayerProvider.Supply += PlayerProvider_Supply;
        }

        void PlayerProvider_Supply(Regulus.Project.SamebestKeys.IPlayer obj)
        {
            _Player = obj;
            _Player.SetPosition(Regulus.Utility.Random.Next(0 , 30), Regulus.Utility.Random.Next(0, 30));
            _Player.SetSpeed(1);
            _Player.Walk(Regulus.Utility.Random.Next(0,360));
            _TimeCounter.Reset();

            _Timeup = Regulus.Utility.Random.Next(1, 10);
        }

        void OnlineProvider_Supply(Regulus.Project.SamebestKeys.IOnline obj)
        {
            _Online = obj;
        }

        public void Leave()
        {
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
                    ResultEvent();
                }
            }
            
        }

        int _Timeup { get; set; }
    }
}
