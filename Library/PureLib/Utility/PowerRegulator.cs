namespace Regulus.Utility
{
	public class PowerRegulator
	{
		private readonly FPSCounter _FPS;

		private readonly int _LowPower;

		private readonly float _Sample;

		private readonly SpinWait _SpinWait;

		private readonly TimeCounter _TimeCount;

		private long _Busy;

		private float _SpinCount;

		private float _WorkCount;

		public int FPS
		{
			get { return _FPS.Value; }
		}

		public float Power { get; private set; }

		public PowerRegulator(int low_power) : this()
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
			_TimeCount = new TimeCounter();
			_FPS = new FPSCounter();
		}

		public void Operate(long busy)
		{
			_FPS.Update();

			if(_Busy <= busy && _FPS.Value > _LowPower)
			{
				_SpinWait.SpinOnce();
				_SpinCount++;
			}
			else
			{
				_SpinWait.Reset();
				_WorkCount++;
			}

			if(_TimeCount.Second > _Sample)
			{
				Power = _WorkCount / (_WorkCount + _SpinCount);

				_WorkCount = 0;
				_SpinCount = 0;
				_TimeCount.Reset();
			}

			_Busy = busy;
		}
	}
}
