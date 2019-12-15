using System;


using Regulus.Utility;

namespace Regulus.Game
{
    public class TimerStage : IStage
    {
        private readonly float _Second;

        public delegate void OnDoneCallback();

        public event OnDoneCallback DoneEvent;

        private readonly Regulus.Utility.TimeCounter _Counter;
        public TimerStage(float second)
        {
            _Counter = new TimeCounter();
            _Second = second;
        }

        void IStage.Enter()
        {
            _Counter.Reset();
        }

        void IStage.Leave()
        {            
        }

        void IStage.Update()
        {
            if(_Counter.Second > _Second)
                DoneEvent();
        }
    }
}