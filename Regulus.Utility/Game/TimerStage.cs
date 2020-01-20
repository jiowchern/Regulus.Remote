using System;


using Regulus.Utility;

namespace Regulus.Game
{
    public class TimerStage : IStatus
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

        void IStatus.Enter()
        {
            _Counter.Reset();
        }

        void IStatus.Leave()
        {            
        }

        void IStatus.Update()
        {
            if(_Counter.Second > _Second)
                DoneEvent();
        }
    }
}