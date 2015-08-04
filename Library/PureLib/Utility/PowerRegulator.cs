// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerRegulator.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the PowerRegulator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
			get { return this._FPS.Value; }
		}

		public float Power { get; private set; }

		public PowerRegulator(int low_power) : this()
		{
			this._LowPower = low_power;
		}

		public PowerRegulator()
		{
			this._Sample = 1.0f;
			this._SpinWait = new SpinWait();
			this._SpinCount = 0;
			this._WorkCount = 0;
			this._Busy = 0;
			this._TimeCount = new TimeCounter();
			this._FPS = new FPSCounter();
		}

		public void Operate(int busy)
		{
			this._FPS.Update();

			if (this._Busy <= busy && this._FPS.Value > this._LowPower)
			{
				this._SpinWait.SpinOnce();
				this._SpinCount++;
			}
			else
			{
				this._SpinWait.Reset();
				this._WorkCount++;
			}

			if (this._TimeCount.Second > this._Sample)
			{
				this.Power = this._WorkCount / (this._WorkCount + this._SpinCount);

				this._WorkCount = 0;
				this._SpinCount = 0;
				this._TimeCount.Reset();
			}

			this._Busy = busy;
		}
	}
}