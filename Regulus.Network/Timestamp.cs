using Stopwatch = System.Diagnostics.Stopwatch;

namespace Regulus.Network
{
    public class Timestamp
    {        
        public readonly long Ticks;
        public readonly long DeltaTicks;

        //public static readonly ITime Time = new Time();
        public static readonly long OneSecondTicks = Stopwatch.Frequency;

        public Timestamp(long ticks, long delta_ticks)
        {            
            Ticks = ticks;
            DeltaTicks = delta_ticks;
        }

        
        
    }
}