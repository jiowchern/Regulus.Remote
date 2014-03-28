using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    class BotEmoStage : Regulus.Game.IStage
    {
        private Regulus.Project.SamebestKeys.IPlayer _Player;
        private Regulus.Project.SamebestKeys.IObservedAbility _Observed;
        Regulus.Utility.TimeCounter _TimeCounter;

        float _Timeup;

        public event Action DoneEvent;
        public BotEmoStage(Regulus.Project.SamebestKeys.IPlayer _Player, Regulus.Project.SamebestKeys.IObservedAbility _Observed)
        {
            _TimeCounter = new Regulus.Utility.TimeCounter();
            this._Player = _Player;
            this._Observed = _Observed;
        }

        void Regulus.Game.IStage.Enter()
        {
            var len = Enum.GetValues(typeof(Regulus.Project.SamebestKeys.ActionStatue)).Length;
            _Player.BodyMovements((Regulus.Project.SamebestKeys.ActionStatue)len);
            _Timeup = Regulus.Utility.Random.Next(3,30);
        }

        void Regulus.Game.IStage.Leave()
        {
                     
        }

        void Regulus.Game.IStage.Update()
        {
            if (_TimeCounter.Second > _Timeup)
                DoneEvent();
        }
    }
}
