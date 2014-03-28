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
        public event Action DoneEvent;
        public BotRunStage(Regulus.Project.SamebestKeys.IPlayer _Player, Regulus.Project.SamebestKeys.IObservedAbility _Observed)
        {
            // TODO: Complete member initialization
            this._Player = _Player;
            this._Observed = _Observed;
        }
        

        
        void Regulus.Game.IStage.Enter()
        {
            _Player.SetSpeed(2);
            _Player.Walk(Regulus.Utility.Random.Next(0,360));
            _Observed.ShowActionEvent += _Observed_ShowActionEvent;
        }

        void _Observed_ShowActionEvent(Regulus.Project.SamebestKeys.Serializable.MoveInfomation obj)
        {
            if (obj.ActionStatue == Regulus.Project.SamebestKeys.ActionStatue.Idle && obj.Speed == 0 && DoneEvent != null)
            {
                DoneEvent();
            }
        }

        void Regulus.Game.IStage.Leave()
        {
            _Observed.ShowActionEvent -= _Observed_ShowActionEvent;
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
