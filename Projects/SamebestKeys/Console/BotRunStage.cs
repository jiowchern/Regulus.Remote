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
        public BotRunStage(Regulus.Project.SamebestKeys.IPlayer _Player, Regulus.Project.SamebestKeys.IObservedAbility _Observed)
        {
            _TimeUp = new Regulus.Utility.TimeCounter();
            this._Player = _Player;
            this._Observed = _Observed;
        }
        

        
        void Regulus.Game.IStage.Enter()
        {
            _Player.SetSpeed(7);
            _Player.Walk(Regulus.Utility.Random.Next(0,360),0);
            _Observed.ShowActionEvent += _Observed_ShowActionEvent;
            _Second = Regulus.Utility.Random.Next(3 , 60);
        }

        void _Observed_ShowActionEvent(Regulus.Project.SamebestKeys.Serializable.MoveInfomation obj)
        {
            if (obj.ActionStatue == Regulus.Project.SamebestKeys.ActionStatue.Idle_1 && obj.Speed == 0 && DoneEvent != null)
            {
                _Player.Walk(Regulus.Utility.Random.Next(0, 360),0);
            }
        }

        void Regulus.Game.IStage.Leave()
        {
            _Observed.ShowActionEvent -= _Observed_ShowActionEvent;
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
