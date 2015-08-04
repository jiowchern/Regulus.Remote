// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Updater.cs" company="">
//   
// </copyright>
// <summary>
//   更新器
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

using Regulus.Framework;

#endregion

namespace Regulus.Utility
{
	/// <summary>
	///     更新器
	/// </summary>
	public interface IUpdatable : IBootable
	{
		bool Update();
	}

	public interface IUpdatable<T> : IBootable
	{
		bool Update(T arg);
	}

	public class Launcher<T> where T : IBootable
	{
		private readonly Queue<T> _Adds = new Queue<T>();

		private readonly Queue<T> _Removes = new Queue<T>();

		private readonly List<T> _Ts = new List<T>();

		public int Count
		{
			get { return this._Ts.Count; }
		}

		private T[] _Objects
		{
			get { return this._Ts.ToArray(); }
		}

		protected IEnumerable<T> _GetObjectSet()
		{
			lock (this._Ts)
			{
				lock (this._Removes)
					this._Remove(this._Removes, this._Ts);

				lock (this._Adds)
					this._Add(this._Adds, this._Ts);

				return this._Objects;
			}
		}

		public void Add(T framework)
		{
			lock (this._Adds)
				this._Adds.Enqueue(framework);
		}

		public void Remove(T framework)
		{
			lock (this._Removes)
				this._Removes.Enqueue(framework);
		}

		private void _Add(Queue<T> add_frameworks, List<T> frameworks)
		{
			while (add_frameworks.Count > 0)
			{
				var fw = add_frameworks.Dequeue();
				frameworks.Add(fw);
				fw.Launch();
			}
		}

		private void _Remove(Queue<T> remove_framework, List<T> frameworks)
		{
			while (remove_framework.Count > 0)
			{
				var fw = remove_framework.Dequeue();
				frameworks.Remove(fw);
				fw.Shutdown();
			}
		}

		public void Shutdown()
		{
			lock (this._Ts)
			{
				this._Shutdown(this._Ts);
				this._Ts.Clear();
			}
		}

		private void _Shutdown(List<T> frameworks)
		{
			foreach (var framework in frameworks)
			{
				framework.Shutdown();
			}
		}
	}

	public class UpdaterToGenerics<T> : Launcher<IUpdatable<T>>
	{
		public void Working(T arg)
		{
			foreach (var t in this._GetObjectSet())
			{
				if (t.Update(arg) == false)
				{
					this.Remove(t);
				}
			}
		}
	}

	public class Updater : Launcher<IUpdatable>
	{
		public void Working()
		{
			foreach (var t in this._GetObjectSet())
			{
				if (t.Update() == false)
				{
					this.Remove(t);
				}
			}
		}
	}
}