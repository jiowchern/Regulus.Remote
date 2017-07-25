namespace Regulus.Network
{
    public class Timestamp
    {        
        public readonly long Ticks;
        public readonly long DeltaTicks;
        public static long _OneSeconds;
        public static long OneSecondTicks { get { return _OneSeconds; } set { _OneSeconds = value; } }

        public Timestamp(long ticks, long delta_ticks)
        {            
            Ticks = ticks;
            DeltaTicks = delta_ticks;
        }

        
    }
}