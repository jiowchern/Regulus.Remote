using Regulus.Utility;
using System;

namespace Regulus.Remote
{
    class Ping
    {
        private readonly float _Interval;

        readonly TimeCounter _TimeCounter;

        float _Seconds;
        bool _Trigger;
        public Ping(float interval)
        {
            _TimeCounter = new TimeCounter();
            this._Interval = interval;
            _Trigger = false;
            TriggerEvent += () => { };
        }



        public float GetSeconds()
        {
            return _GetSeconds();
        }
        private float _GetSeconds()
        {
            if(_Trigger == false&& _TimeCounter.Second > _Interval)
            {
                _TimeCounter.Reset();
                _Trigger = true;

                TriggerEvent();
                
            }
            return _Seconds;
        }

        public event System.Action TriggerEvent;

        internal void Update()
        {
            _Trigger = false;
            _Seconds = _TimeCounter.Second;
        }
    }
}
