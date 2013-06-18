using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting
{
    public interface ITime
    {
        Value<long> GetTick();        
    }


    public class TimeCounter
    {

        long _Ticks;
        internal void Reset()
        {
            _Ticks = System.DateTime.Now.Ticks;
        }

        public long Ticks { get 
        {
            return System.DateTime.Now.Ticks - _Ticks;
        } }
    }

    public class Timer
    {
        private long _Current;
        Action<long> _TimeUp;
        long _Interval;
        public Timer(System.TimeSpan interval , Action<long> time_up )
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

    public class Time : ITime 
    {
        
        ITime _Time;
        private long _Current;
        private long _Real;
        public long Ticks { get { return _Real; } }
        
        Timer _TimingTimer;

        public Time()
        {
            _Current = System.DateTime.Now.Ticks;
            _Real = _Current;
            _TimingTimer = new Timer(new TimeSpan(0, 1, 0), (current) =>
            {
                
                if (_Time != null)
                {
                    _Request.Reset();
                    _Time.GetTick().OnValue += _Timing;
                }
            });
        }

        TimeCounter _Request = new TimeCounter();
        public Time(ITime time) : this()
        {
            if (time != null)
            {                
                _Time = time;
                _Request.Reset();
                _Time.GetTick().OnValue += _Timing;                
            }
        }

        void _Timing(long current)
        {
            _Real = current + _Request.Ticks;
        }
        
        public void Update()
        {
            var current = System.DateTime.Now.Ticks;
            Delta = current - _Current;
            _Real += Delta;
            _TimingTimer.Update(Delta);            
            _Current = current;
        }
        
        ~Time()
        {
            if (_Time != null)
            {
                
            }            
        }

        public long Delta { get; private set; }

        Value<long> ITime.GetTick()
        {
            return _Real;
        }
    }
}
