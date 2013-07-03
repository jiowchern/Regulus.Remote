
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{

    public interface ITime
    {
        Value<long> GetTick();
    }

    public class Time : Regulus.Utility.Time ,ITime
    {
        ITime _Time;
        Regulus.Utility.Timer _TimingTimer;

        public Time(ITime time) : this()
        {
            if (time != null)
            {
                _Time = time;
                _Request.Reset();
                _Time.GetTick().OnValue += _Timing;
            }

        }
        public Time() : base()
        {
            _TimingTimer = new Regulus.Utility.Timer(new TimeSpan(0, 1, 0), (current) =>
            {
                if (_Time != null)
                {
                    _Request.Reset();
                    _Time.GetTick().OnValue += _Timing;
                }
            });
        }
        Value<long> ITime.GetTick()
        {
            return _Real;
        }
        new void Update()
        {
            base.Update();
            _TimingTimer.Update(Delta);
            
        }
        void _Timing(long current)
        {
            _Real = current + _Request.Ticks;
        }
    }
}
