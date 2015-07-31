// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Time.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the FPSCounter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Framework;

#endregion

namespace Regulus.Utility
{
	public class FPSCounter
	{
		private readonly TimeCounter _Counter;

		private int _Frames;

		public int Value { get; private set; }

		public FPSCounter()
		{
			this._Counter = new TimeCounter();
		}

		public void Update()
		{
			this._Frames++;
			if (this._Counter.Second > 1.0f)
			{
				this.Value = this._Frames;
				this._Frames = 0;
				this._Counter.Reset();
			}
		}
	}


	public class TimeCounter
	{
		private long _Begin;

		public long Ticks
		{
			get { return DateTime.Now.Ticks - this._Begin; }
		}

		public float Second
		{
			get { return (float)new TimeSpan(this.Ticks).TotalSeconds; }
		}

		public TimeCounter()
		{
			this.Reset();
		}

		public void Reset()
		{
			this._Begin = DateTime.Now.Ticks;
		}
	}

	public class Stopwatch
	{
		private TimeCounter _Current;

		private long _Interval;

		private bool _Pause;

		private TimeCounter _Stop;

		private long _StopTick;

		public long Ticks
		{
			get
			{
				if (this._Pause == false)
				{
					return this._Current.Ticks - this._Interval;
				}

				return this._StopTick;
			}
		}

		public void Reset()
		{
			this._Current.Reset();
			this._Stop.Reset();
			this._Interval = 0;
			this._StopTick = 0;
		}

		public void Continue()
		{
			this._Pause = false;
			this._Interval += this._Stop.Ticks;
		}

		public void Stop()
		{
			this._Pause = true;
			this._Stop.Reset();
			this._StopTick = this._Current.Ticks;
		}
	}

	public class Timer
	{
		private readonly long _Interval;

		private readonly Action<long> _TimeUp;

		private long _Current;

		public Timer(float second, Action<long> time_up) : this(TimeSpan.FromSeconds(second), time_up)
		{
		}

		public Timer(TimeSpan interval, Action<long> time_up)
		{
			this._TimeUp = time_up;
			this._Interval = interval.Ticks;
			this._Current = 0;
		}

		protected bool _Update(long delta)
		{
			var newTime = this._Current + delta;

			if (newTime > this._Interval)
			{
				this._Current = newTime - this._Interval;
				this._TimeUp(this._Current);
				return true;
			}

			this._Current = newTime;
			return false;
		}

		public void Update(long delta)
		{
			this._Update(delta);
		}
	}

	public class Task : Timer, IUpdatable<long>
	{
		public Task(TimeSpan interval, Action<long> time_up)
			: base(interval, time_up)
		{
		}

		bool IUpdatable<long>.Update(long arg)
		{
			return this._Update(arg) == false;
		}

		void IBootable.Launch()
		{
		}

		void IBootable.Shutdown()
		{
		}
	}

	public class Scheduler : Singleton<Scheduler>
	{
		private readonly UpdaterToGenerics<long> _Tasks;

		private readonly Time _Time;

		public Scheduler()
		{
			this._Tasks = new UpdaterToGenerics<long>();
			this._Time = new Time();
		}

		public Task Add(TimeSpan interval, Action<long> time_up)
		{
			var task = new Task(interval, time_up);
			this._Tasks.Add(task);
			return task;
		}

		public void Remove(Task task)
		{
			this._Tasks.Remove(task);
		}

		public void Update()
		{
			this._Time.Update();
			this._Tasks.Working(this._Time.Delta);
		}
	}

	public class Time
	{
		private long _Current;

		protected long _Real;

		protected TimeCounter _Request = new TimeCounter();

		/// <summary>
		///     目前時間Ticks
		/// </summary>
		public long Ticks
		{
			get { return this._Real; }
		}

		public long Delta { get; private set; }

		public float DeltaSecond
		{
			get { return (float)new TimeSpan(this.Delta).TotalSeconds; }
		}

		public Time()
		{
			this._Current = DateTime.Now.Ticks;
			this._Real = this._Current;
		}

		public void Update()
		{
			var current = DateTime.Now.Ticks;
			this.Delta = current - this._Current;
			this._Real += this.Delta;

			this._Current = current;
		}

		~Time()
		{
		}
	}

	public class IndependentTimer
	{
		private readonly Time _Time;

		private readonly Timer _Timer;

		public IndependentTimer(TimeSpan interval, Action<long> time_up)
		{
			this._Timer = new Timer(interval, time_up);
			this._Time = new Time();
		}

		public void Update()
		{
			this._Time.Update();
			this._Timer.Update(this._Time.Delta);
		}
	}
}