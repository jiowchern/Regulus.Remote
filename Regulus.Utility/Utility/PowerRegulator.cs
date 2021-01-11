using System.Threading;

namespace Regulus.Utility
{
    public class PowerRegulator
    {
        private readonly FPSCounter _FPS;

        private readonly int _LowPower;


        private SpinWait _SpinWait;


        private long _Busy;

        private long _SpinCount;

        private long _WorkCount;

        public int FPS
        {
            get { return _FPS.Value; }
        }

        public float Power => _GetSample();

        private float _GetSample()
        {
            long count = _WorkCount + _SpinCount;
            if (count == 0)
                return 0;

            double power = _WorkCount / (double)count;
            return (float)power;
        }

        public PowerRegulator(int low_power) : this()
        {
            _LowPower = low_power;
        }

        public PowerRegulator()
        {

            _SpinWait = new SpinWait();
            _SpinCount = 0;
            _WorkCount = 0;
            _Busy = 0;

            _FPS = new FPSCounter();
        }

        public void Operate(long busy)
        {
            _SpinWait.SpinOnce();
            _FPS.Update();

            if (_Busy <= busy && _FPS.Value > _LowPower)
            {


                _SpinWait.SpinOnce();
                
                _SpinCount++;
            }
            else
            {
                _SpinWait.Reset();
                _WorkCount++;
            }
            _Busy = busy;
        }
    }
}
