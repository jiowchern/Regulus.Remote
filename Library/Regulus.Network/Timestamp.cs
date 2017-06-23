namespace Regulus.Network
{
    public class Timestamp
    {
        public static readonly long OneSecondTicks = System.TimeSpan.FromSeconds(1).Ticks;        
        public readonly long Ticks;
        public readonly long DeltaTicks;

        public Timestamp(long ticks, long delta_ticks)
        {
            Ticks = ticks;
            DeltaTicks = delta_ticks;
        }
    }
}