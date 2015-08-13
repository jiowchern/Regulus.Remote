using System;


using Regulus.Utility;

namespace Regulus.Remoting
{
	public interface ITime
	{
		Value<long> GetTick();
	}

	public class Time : Utility.Time, ITime
	{
		private readonly ITime _Time;

		private readonly Timer _TimingTimer;

		public Time(ITime time) : this()
		{
			if(time != null)
			{
				_Time = time;
				_Request.Reset();
				_Time.GetTick().OnValue += _Timing;
			}
		}

		public Time()
		{
			_TimingTimer = new Timer(
				new TimeSpan(0, 1, 0), 
				current =>
				{
					if(_Time != null)
					{
						_Request.Reset();
						_Time.GetTick().OnValue += _Timing;
					}
				});
		}

		Value<long> ITime.GetTick()
		{
			return _Real;
		}

		private new void Update()
		{
			base.Update();
			_TimingTimer.Update(Delta);
		}

		private void _Timing(long current)
		{
			_Real = current + _Request.Ticks;
		}
	}
}
