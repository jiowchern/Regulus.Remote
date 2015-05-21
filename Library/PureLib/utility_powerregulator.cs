using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class PowerRegulator
    {
        float _Sample;
        float _SpinCount = 0;
        float _WorkCount = 0;
        
        long _Busy = 0;
        Regulus.Utility.TimeCounter _TimeCount ;
        Regulus.Utility.SpinWait _SpinWait;
        Regulus.Utility.FPSCounter _FPS;
        public int FPS { get { return _FPS.Value; } }

        public float Power { get; private set; }

        public PowerRegulator(int low_power) :this()
        {
            _LowPower = low_power;
        }

        public PowerRegulator()
        {
            _Sample = 1.0f;
            _SpinWait = new SpinWait();
            _SpinCount = 0;
            _WorkCount = 0;
            _Busy = 0;            
            _TimeCount = new Utility.TimeCounter();
            _FPS = new FPSCounter();
        }

        public void Operate(int busy)
        {
            _FPS.Update();

            if (_Busy <= busy && _FPS.Value > _LowPower )
            {
                _SpinWait.SpinOnce();
                _SpinCount++;
            }
            else
            {
                _SpinWait.Reset();
                _WorkCount++;
            }

            if (_TimeCount.Second > _Sample)
            {
                Power = _WorkCount / (_WorkCount + _SpinCount);
                
                _WorkCount = 0;
                _SpinCount = 0;
                _TimeCount.Reset();
            }

            _Busy = busy;
        }

        private int _LowPower;
    }
}
