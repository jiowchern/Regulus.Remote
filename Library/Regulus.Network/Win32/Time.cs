using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Regulus.Network.Win32
{
    public class Time : ITime
    {
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);

        private static long _OneSecondTicks;
        private long _Last;
        private long _Delta;

        public static long OneSecondTicks
        {
            get
            {
                if (_OneSecondTicks == 0)
                {
                    QueryPerformanceFrequency(ref _OneSecondTicks);
                }
                return _OneSecondTicks;
            }
        }

        public Time()
        {
            long now = 0;
            QueryPerformanceCounter(ref now);
            _Last = now;
            _Delta = 0;
        }

        void ITime.Sample()
        {
            long now = 0;
            QueryPerformanceCounter(ref now);
            var delta = now - _Last;
            _Delta = delta;
            _Last = now;

        }

        long ITime.OneSeconds { get{return Regulus.Network.Win32.Time.OneSecondTicks; } }
        long ITime.Now { get { return _Last; } }
        long ITime.Delta { get { return _Delta; } }
    }
}
