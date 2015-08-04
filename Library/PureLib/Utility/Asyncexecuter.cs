// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Asyncexecuter.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AsyncExecuter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Threading;

using Regulus.Collection;

#endregion

namespace Regulus.Utility
{
	public class AsyncExecuter
	{
		private readonly Queue<Action> _Tasks;

		private volatile int _Count;

		public AsyncExecuter(params Action[] callbacks)
		{
			this._Tasks = new Queue<Action>();

			foreach (var task in callbacks)
			{
				this.Push(task);
			}
		}

		public void WaitDone()
		{
			while (this.IsDone() == false)
			{
				;
			}
		}

		internal bool IsDone()
		{
			lock (this)
				return this._Count == 0;
		}

		public void Push(Action callback)
		{
			var execute = false;
			lock (this)
			{
				execute = this._Count == 0;
				this._Count++;
				this._Tasks.Enqueue(callback);
			}

			if (execute)
			{
				this._Execute();
			}
		}

		private void _Execute()
		{
			Action task;
			if (this._Tasks.TryDequeue(out task))
			{
				ThreadPool.QueueUserWorkItem(this._Run, task);
			}
		}

		private void _Run(object state)
		{
			var task = (Action)state;
			task();
			lock (this)
				this._Count--;
			this._Execute();
		}
	}
}