using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{


	public class FPSCounter
	{
		public int Value {  get; private set;}
		TimeCounter _Counter;
		int _Frames;
		public FPSCounter()
		{
			_Counter = new TimeCounter();
		}

		public void Update()
		{
			_Frames++;
			if (_Counter.Second > 1.0f)
			{
				Value = _Frames;
				_Frames = 0;
				_Counter.Reset();
			}
		}
	}


    public class TimeCounter
    {
        
        long _Begin;
        
        public TimeCounter()
        {
            
            Reset();
        }
        public void Reset()
        {
            _Begin = System.DateTime.Now.Ticks;
        }

        public long Ticks
        {
            get
            {
                return System.DateTime.Now.Ticks - _Begin;
            }
        }

        public float Second
        {
            get
            {
                return (float)new System.TimeSpan(Ticks).TotalSeconds;
            }
        }
    }

    public class Stopwatch
    {
        TimeCounter _Current;
        TimeCounter _Stop;
        long _Interval;
        long _StopTick;
        bool _Pause;

        public void Reset()
        {
            _Current.Reset();
            _Stop.Reset();
            _Interval = 0;
            _StopTick = 0;
        }

        public void Continue()
        {
            _Pause = false;
            _Interval += _Stop.Ticks;
        }
        public void Stop()
        {
            _Pause = true;
            _Stop.Reset();
            _StopTick = _Current.Ticks;
        }
        public long Ticks
        {
            get
            {
                if (_Pause == false)
                    return _Current.Ticks - _Interval;
                else
                    return _StopTick;

            }
        }
    }

    public class Timer 
    {
        private long _Current;
        Action<long> _TimeUp;
        long _Interval;
        public Timer(float second, Action<long> time_up) : this( System.TimeSpan.FromSeconds(second) , time_up)
        { 

        }
        public Timer(System.TimeSpan interval, Action<long> time_up)
        {
            _TimeUp = time_up;
            _Interval = interval.Ticks;
            _Current = 0;
        }
        protected bool _Update(long delta)
        {
            var newTime = _Current + delta;

            if (newTime > _Interval)
            {
                _Current = newTime - _Interval;
                _TimeUp(_Current);
                return true;
            }
            else
                _Current = newTime;
            return false;
        }

        public void Update(long delta)
        {
            _Update(delta);
        }
    }

    public class Task : Timer , Regulus.Utility.IUpdatable<long>
    {
        public Task(System.TimeSpan interval, Action<long> time_up)
            : base(interval, time_up)
        { 

        }

        bool IUpdatable<long>.Update(long arg)
        {
            return _Update(arg) == false;
        }

        void Framework.IBootable.Launch()
        {
            
        }

        void Framework.IBootable.Shutdown()
        {
            
        }
    }

    public class Scheduler : Regulus.Utility.Singleton<Scheduler> 
    {
        Time _Time;
        Regulus.Utility.CenterOfUpdateableToGenerics<long> _Tasks;
        public Scheduler()
        {
            _Tasks = new CenterOfUpdateableToGenerics<long>();
            _Time = new Time();
        }

        public Task Add(System.TimeSpan interval, Action<long> time_up)
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

		/// <summary>
		/// 目前時間Ticks
		/// </summary>
        public long Ticks { get { return _Real; } }

        public Time()
        {
            _Current = System.DateTime.Now.Ticks;
            _Real = _Current;            
        }

        protected TimeCounter _Request = new TimeCounter();

        public void Update()
        {
            var current = System.DateTime.Now.Ticks;
            Delta = current - _Current;
            _Real += Delta;
            
            _Current = current;
        }

        ~Time()
        {

        }

        public long Delta { get; private set; }
        public float DeltaSecond { get 
        {
            return (float)new System.TimeSpan(Delta).TotalSeconds;
        } }

    }

    public class IndependentTimer  
    {
        Timer _Timer;
        Time _Time;

        public IndependentTimer(System.TimeSpan interval, Action<long> time_up)
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
