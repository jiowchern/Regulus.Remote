using Regulus.Framework;
using Regulus.Remoting.Ghost.Native;
using Regulus.Utility;

namespace FormulaUserBot
{
	internal class PacketRegulator : IUpdatable
	{
		private readonly TimeCounter _Counter;

		private long _PrevSampling;

		private long _PrevSamplingCount;

		private long _Sampling;

		private long _SamplingCount;

		public double Sampling
		{
			get { return _PrevSampling / (double)_PrevSamplingCount; }
		}

		public PacketRegulator()
		{
			_Counter = new TimeCounter();
		}

		bool IUpdatable.Update()
		{
			_Sampling += Agent.RequestPackages + Agent.ResponsePackages;
			_SamplingCount++;

			if(_Counter.Second > 0.016f)
			{
				if(_PrevSampling < _Sampling)
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

		void IBootable.Launch()
		{
		}

		void IBootable.Shutdown()
		{
		}
	}
}
