using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Lockstep
{
    public class Propeller
    {
        private readonly long _Interval;
        private long _Ticks;
        private int _Count;

        public Propeller(long interval)
        {
            _Interval = interval;
        }

        public void Heartbeat()
        {
            _Count++;
        }
        public bool Propel(long delta)
        {
            _Ticks += delta;
            if (_Ticks >= _Interval && _Count > 0)
            {
                --_Count;
                _Ticks -= _Interval;

                return true;
            }

            return false;
        }
    }
}
