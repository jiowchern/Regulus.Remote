using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotRunStage : Regulus.Game.IStage
    {
        private Regulus.Project.SamebestKeys.IPlayer _Player;
        private Regulus.Project.SamebestKeys.IObservedAbility _Observed;
        Regulus.Utility.TimeCounter _TimeUp;
        float _Second;
        public event Action DoneEvent;
        private Regulus.Project.SamebestKeys.IUser _User;
        private Regulus.Types.Point? _BornPoint;
        public BotRunStage()
        {
            _TimeUp = new Regulus.Utility.TimeCounter();            
        }

        public BotRunStage(Regulus.Project.SamebestKeys.IUser _User, Regulus.Types.Point? _BornPoint) :this()
        {
           
            this._User = _User;
            this._BornPoint = _BornPoint;
        }
        

        
        void Regulus.Game.IStage.Enter()
        {
            _User.PlayerProvider.Supply += PlayerProvider_Supply;
            
        }

        void PlayerProvider_Supply(Regulus.Project.SamebestKeys.IPlayer obj)
        {
            _Player = obj;
            _User.ObservedAbilityProvider.Supply += ObservedAbilityProvider_Supply;
        }

        void ObservedAbilityProvider_Supply(Regulus.Project.SamebestKeys.IObservedAbility obj)
        {
            if (obj.Id == _Player.Id)
            {
                _Observed = obj;
                _Observed.ShowActionEvent += _Observed_ShowActionEvent;

                _Run();
            }
        }

        private void _Run()
        {
            //if (_BornPoint.HasValue)
                //_Player.SetPosition(_BornPoint.Value.X, _BornPoint.Value.Y);

            _Player.SetVision(10);
            _Player.SetEnergy(1000);
            _Player.SetSpeed(5);
            _Player.Walk(Regulus.Utility.Random.Next(0, 360), 0);            
            _Second = Regulus.Utility.Random.Next(5, 10);
        }

        void _Observed_ShowActionEvent(Regulus.Project.SamebestKeys.Serializable.MoveInfomation obj)
        {
            if (obj.Speed == 0)
            {
                _Player.Walk(Regulus.Utility.Random.Next(0, 360),0);
            }
        }

        void Regulus.Game.IStage.Leave()
        {
            if (_Observed != null)
                _Observed.ShowActionEvent -= _Observed_ShowActionEvent;
            _User.PlayerProvider.Supply -= PlayerProvider_Supply;
            _User.ObservedAbilityProvider.Supply -= ObservedAbilityProvider_Supply;
        }

        void Regulus.Game.IStage.Update()
        {
            if(_TimeUp.Second > _Second)
            {
                DoneEvent();
            }
        }
    }
}
