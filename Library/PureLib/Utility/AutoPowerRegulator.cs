namespace Regulus.Utility
{
    public class AutoPowerRegulator
    {
        private TimeCounter _Counter;
        private PowerRegulator _PowerRegulator;

        private long _PreviousTicks;

        public AutoPowerRegulator(PowerRegulator power_regulator)
        {
            _Counter = new TimeCounter();
            _PreviousTicks = _Counter.Ticks;
            _PowerRegulator = power_regulator;
        }

        public void Operate()
        {
            var ticks = _Counter.Ticks;
            _PowerRegulator.Operate(ticks - _PreviousTicks);
            _PreviousTicks = ticks;
        }
    }
}