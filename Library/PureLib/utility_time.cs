using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    


    public class TimeCounter
    {

        long _Ticks;
        public TimeCounter()
        {
            _Ticks = System.DateTime.Now.Ticks;
        }
        public void Reset()
        {
            _Ticks = System.DateTime.Now.Ticks;
        }

        public long Ticks
        {
            get
            {
                return System.DateTime.Now.Ticks - _Ticks;
            }
        }
    }

    public class Timer
    {
        private long _Current;
        Action<long> _TimeUp;
        long _Interval;
        public Timer(System.TimeSpan interval, Action<long> time_up)
        {
            _TimeUp = time_up;
            _Interval = interval.Ticks;
            _Current = 0;
        }
        public void Update(long delta)
        {
            var newTime = _Current + delta;

            if (newTime > _Interval)
            {
                _Current = newTime - _Interval;
                _TimeUp(_Current);
            }
            else
                _Current = newTime;
        }
    }

    public class Time 
    {

        
        private long _Current;
        protected long _Real;
        public long Ticks { get { return _Real; } }

        

        public Time()
        {
            _Current = System.DateTime.Now.Ticks;
            _Real = _Current;
            
        }

        protected TimeCounter _Request = new TimeCounter();

        

        public void Update()
        {
            var current = System.DateTime.Now.Ticks;
            Delta = current - _Current;
            _Real += Delta;
            
            _Current = current;
        }

        ~Time()
        {

        }

        public long Delta { get; private set; }

    }
}
