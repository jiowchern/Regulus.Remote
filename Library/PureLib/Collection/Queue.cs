// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Queue.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Queue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Regulus.Collection
{
	public class Queue<T>
	{
		private readonly System.Collections.Generic.Queue<T> _Set;

		public Queue()
		{
			this._Set = new System.Collections.Generic.Queue<T>();
		}

		public Queue(params T[] objs)
		{
			this._Set = new System.Collections.Generic.Queue<T>(objs);
		}

		public void Enqueue(T package)
		{
			lock (this._Set)
			{
				this._Set.Enqueue(package);
			}
		}

		public bool TryDequeue(out T obj)
		{
			lock (this._Set)
			{
				if (this._Set.Count > 0)
				{
					obj = this._Set.Dequeue();
					return true;
				}
			}

			obj = default(T);
			return false;
		}

		public T[] DequeueAll()
		{
			lock (this._Set)
			{
				var all = this._Set.ToArray();
				this._Set.Clear();
				return all;
			}
		}
	}
}