namespace Regulus.Utility
{
    public class AutoPowerRegulator
    {
        private readonly TimeCounter _Counter;
        private readonly PowerRegulator _PowerRegulator;

        private long _PreviousTicks;
        readonly System.Threading.SpinWait _Spin;
        public AutoPowerRegulator(PowerRegulator power_regulator)
        {
            _Spin = new System.Threading.SpinWait();
            /*_Counter = new TimeCounter();
            _PreviousTicks = _Counter.Ticks;
            _PowerRegulator = power_regulator;*/
        }

        public void Operate()
        {
            _Spin.SpinOnce();
            /*long ticks = _Counter.Ticks;
            _PowerRegulator.Operate(ticks - _PreviousTicks);
            _PreviousTicks = ticks;*/
        }
    }
}