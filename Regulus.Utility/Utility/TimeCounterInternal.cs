using System.Diagnostics;
using System.Threading;

namespace Regulus.Utility
{
    public class TimeCounterInternal
    {
        private readonly Stopwatch _Stopwatch;
        private SpinLock _Lock;
        public TimeCounterInternal()
        {
            _Stopwatch = Stopwatch.StartNew();

            _Lock = new SpinLock();
            _Ticks = _Stopwatch.ElapsedTicks;
        }



        private long _Ticks;

        public long Ticks
        {
            get
            {
                bool lockTaken = false;
                try
                {
                    _Lock.Enter(ref lockTaken);
                    _Ticks = _Stopwatch.ElapsedTicks;
                }
                finally
                {
                    if (lockTaken)
                        _Lock.Exit(false);
                }
                return _Ticks;
            }
        }

        public long Frequency { get { return Stopwatch.Frequency; } }
    }
}