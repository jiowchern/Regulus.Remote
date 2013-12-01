using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    


    public class TimeCounter
    {
        
        long _Begin;
        
        public TimeCounter()
        {
            
            Reset();
        }
        public void Reset()
        {
            _Begin = System.DateTime.Now.Ticks;
        }

        public long Ticks
        {
            get
            {
                return System.DateTime.Now.Ticks - _Begin;
            }
        }

        public float Second
        {
            get
            {
                return (float)new System.TimeSpan(Ticks).TotalSeconds;
            }
        }
    }

    public class Stopwatch
    {
        TimeCounter _Current;
        TimeCounter _Stop;
        long _Interval;
        long _StopTick;
        bool _Pause;

        public void Reset()
        {
            _Current.Reset();
            _Stop.Reset();
            _Interval = 0;
            _StopTick = 0;
        }

        public void Continue()
        {
            _Pause = false;
            _Interval += _Stop.Ticks;
        }
        public void Stop()
        {
            _Pause = true;
            _Stop.Reset();
            _StopTick = _Current.Ticks;
        }
        public long Ticks
        {
            get
            {
                if (_Pause == false)
                    return _Current.Ticks - _Interval;
                else
                    return _StopTick;

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
        public float DeltaSecond { get 
        {
            return (float)new System.TimeSpan(Delta).TotalSeconds;
        } }

    }

    public class IndependentTimer  
    {
        Timer _Timer;
        Time _Time;

        public IndependentTimer(System.TimeSpan interval, Action<long> time_up)
        {
            _Timer = new Timer(interval, time_up);
            _Time = new Time();
        }

        public void Update()
        {
            _Time.Update();
            _Timer.Update(_Time.Delta);
        }
    }
}
