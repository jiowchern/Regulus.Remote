namespace Regulus.Utility
{
    public class TimeCounter
    {
        private readonly static TimeCounterInternal _TimeCounterInternal = new TimeCounterInternal();
        private long _Last;
        public long Ticks
        {
            get { return _GetTicks(); }
        }

        private long _GetTicks()
        {
            return _TimeCounterInternal.Ticks - _Last;
        }

        public static long SecondTicks
        {
            get { return _TimeCounterInternal.Frequency; }
    }
        public float Second
        {
            get { return (_GetTicks() / (float)_TimeCounterInternal.Frequency); }
        }

        public TimeCounter()
        {
		    
            
		    

            Reset();
        }

        public void Reset()
        {
            _Last = _TimeCounterInternal.Ticks;            
        }

        public static double GetSeconds(long ticks)
        {
            return ticks / (double)_TimeCounterInternal.Frequency;
        }
    }
}