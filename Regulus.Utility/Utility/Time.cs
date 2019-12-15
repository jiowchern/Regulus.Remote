using System;
using Regulus.Framework;

namespace Regulus.Utility
{
	public class FPSCounter
	{
		private readonly TimeCounter _Counter;

		private int _Frames;

		public int Value { get; private set; }

		public FPSCounter()
		{
			_Counter = new TimeCounter();
		}

		public void Update()
		{
			_Frames++;
			if(_Counter.Second > 1.0f)
			{
				Value = _Frames;
				_Frames = 0;
				_Counter.Reset();
			}
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
			_TimeUp = time_up;
			_Interval = interval.Ticks;
			_Current = 0;
		}

		protected bool _Update(long delta)
		{
			var newTime = _Current + delta;

			if(newTime > _Interval)
			{
				_Current = newTime - _Interval;
				_TimeUp(_Current);
				return true;
			}

			_Current = newTime;
			return false;
		}

		public void Update(long delta)
		{
			_Update(delta);
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
			return _Update(arg) == false;
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
		private readonly Updater<long> _Tasks;

		private readonly Time _Time;

		public Scheduler()
		{
			_Tasks = new Updater<long>();
			_Time = new Time();
		}

		public Task Add(TimeSpan interval, Action<long> time_up)
		{
			var task = new Task(interval, time_up);
			_Tasks.Add(task);
			return task;
		}

		public void Remove(Task task)
		{
			_Tasks.Remove(task);
		}

		public void Update()
		{
			_Time.Update();
			_Tasks.Working(_Time.Delta);
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
			get { return _Real; }
		}

		public long Delta { get; private set; }

		public float DeltaSecond
		{
			get { return (float)new TimeSpan(Delta).TotalSeconds; }
		}

		public Time()
		{
			_Current = DateTime.Now.Ticks;
			_Real = _Current;
		}

		public void Update()
		{
			var current = DateTime.Now.Ticks;
			Delta = current - _Current;
			_Real += Delta;

			_Current = current;
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
			_Timer = new Timer(interval, time_up);
			_Time = new Time();
		}

		public void Update()
		{
			_Time.Update();
			_Timer.Update(_Time.Delta);
		}
	}
}
