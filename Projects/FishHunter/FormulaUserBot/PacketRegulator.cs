using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormulaUserBot
{
    class PacketRegulator : Regulus.Utility.IUpdatable
    {

        Regulus.Utility.TimeCounter _Counter;

        public double Sampling { get { return _PrevSampling / (double)_PrevSamplingCount; } }
        long _PrevSampling;
        long _PrevSamplingCount;
        long _Sampling;
        long _SamplingCount;

        
        public PacketRegulator()
        {
            _Counter = new Regulus.Utility.TimeCounter();
        }
        bool Regulus.Utility.IUpdatable.Update()
        {
            _Sampling += (Regulus.Remoting.Ghost.Native.Agent.RequestPackages + Regulus.Remoting.Ghost.Native.Agent.ResponsePackages);
            _SamplingCount++;

            if (_Counter.Second > 0.016f)
            {
                if (_PrevSampling < _Sampling)
                {
                    HitHandler.Interval *= 1.01f;
                }
                else
                {
                    HitHandler.Interval *= 0.99f;
                }

                _PrevSampling = _Sampling;
                _PrevSamplingCount = _SamplingCount;

                _Sampling = 0;
                _SamplingCount = 0;

                _Counter.Reset();
            }


            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            
        }
    }
}
