// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Poller.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Poller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;

#endregion

namespace Regulus.Utility
{
	public class Poller<T> where T : class
	{
		private readonly Queue<T> _Adds = new Queue<T>();

		private readonly List<T> _Objects = new List<T>();

		private readonly Queue<Func<T, bool>> _Removes = new Queue<Func<T, bool>>();

		public void Add(T obj)
		{
			lock (this._Adds)
				this._Adds.Enqueue(obj);
		}

		public void Remove(Func<T, bool> obj)
		{
			lock (this._Removes)
				this._Removes.Enqueue(obj);
		}

		public T[] UpdateSet()
		{
			lock (this._Objects)
			{
				lock (this._Adds)
					this._Add(this._Adds);
				lock (this._Removes)
					this._Remove(this._Removes);

				return this._Objects.ToArray();
			}
		}

		private void _Remove(Queue<Func<T, bool>> removes)
		{
			while (removes.Count > 0)
			{
				var obj = removes.Dequeue();

				this._Objects.RemoveAll(o => obj.Invoke(o));
			}
		}

		private void _Add(Queue<T> adds)
		{
			while (adds.Count > 0)
			{
				var obj = adds.Dequeue();
				this._Objects.Add(obj);
			}
		}
	}
}