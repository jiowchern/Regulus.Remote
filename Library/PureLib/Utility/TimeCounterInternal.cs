using System.Diagnostics;
using System.Threading;

namespace Regulus.Utility
{
    public class TimeCounterInternal
    {
        public TimeCounterInternal()
        {
            
            ThreadPool.QueueUserWorkItem(_Run);

        }

        private void _Run(object state)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var wait = new Regulus.Utility.SpinWait();
            while (true)
            {
                _Ticks = stopwatch.ElapsedTicks;
                wait.SpinOnce();
            }
        }

        private long _Ticks;
        public long Ticks { get { return _Ticks; } }
        public long Frequency { get { return Stopwatch.Frequency; } }
    }
}