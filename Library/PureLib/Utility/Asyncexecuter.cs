using System;
using System.Threading;


using Regulus.Collection;

namespace Regulus.Utility
{
	public class AsyncExecuter
	{
		private readonly Queue<Action> _Tasks;

		private volatile int _Count;

		public AsyncExecuter(params Action[] callbacks)
		{
			_Tasks = new Queue<Action>();

			foreach(var task in callbacks)
			{
				Push(task);
			}
		}

		public void WaitDone()
		{
			while(IsDone() == false)
			{
				;
			}
		}

		internal bool IsDone()
		{
			lock(this)
				return _Count == 0;
		}

		public void Push(Action callback)
		{
			var execute = false;
			lock(this)
			{
				execute = _Count == 0;
				_Count++;
				_Tasks.Enqueue(callback);
			}

			if(execute)
			{
				_Execute();
			}
		}


		private void _Execute()
		{
			Action task;
			if(_Tasks.TryDequeue(out task))
			{
				ThreadPool.QueueUserWorkItem(_Run, task);
			}
		}

		private void _Run(object state)
		{
			var task = (Action)state;
			task();
			lock(this)
				_Count--;
			_Execute();
		}
	}
}
