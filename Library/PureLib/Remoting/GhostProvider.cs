// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GhostProvider.cs" company="">
//   
// </copyright>
// <summary>
//   介面物件通知器
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Regulus.Remoting
{
	/// <summary>
	///     介面物件通知器
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface INotifier<T>
	{
		/// <summary>
		///     伺服器端如果有物件傳入則會發生此事件
		///     此事件傳回的物件如果沒有備參考到則會發生Unsupply
		/// </summary>
		event Action<T> Return;

		/// <summary>
		///     伺服器端如果有物件傳入則會發生此事件
		/// </summary>
		event Action<T> Supply;

		/// <summary>
		///     伺服器端如果有物件關閉則會發生此事件
		/// </summary>
		event Action<T> Unsupply;

		/// <summary>
		///     在系統裡的介面物件數量
		/// </summary>
		T[] Ghosts { get; }

		/// <summary>
		///     在系統裡的介面物件數量(弱參考型別)
		/// </summary>
		T[] Returns { get; }
	}

	public interface IProvider
	{
		IGhost[] Ghosts { get; }

		void Add(IGhost entiry);

		void Remove(Guid id);

		IGhost Ready(Guid id);

		void ClearGhosts();
	}


	public class TProvider<T> : INotifier<T>, IProvider
		where T : class
	{
		private event Action<T> _Return;

		private event Action<T> _Supply;

		private event Action<T> _Unsupply;

		private readonly List<T> _Entitys = new List<T>();

		private readonly List<WeakReference> _Returns = new List<WeakReference>();

		private readonly List<T> _Waits = new List<T>();

		event Action<T> INotifier<T>.Supply
		{
			add
			{
				this._Supply += value;

				lock (this._Entitys)
				{
					foreach (var e in this._Entitys.ToArray())
					{
						value(e);
					}
				}
			}

			remove { this._Supply -= value; }
		}

		event Action<T> INotifier<T>.Unsupply
		{
			add { this._Unsupply += value; }
			remove { this._Unsupply -= value; }
		}

		T[] INotifier<T>.Ghosts
		{
			get
			{
				lock (this._Entitys)
					return this._Entitys.ToArray();
			}
		}

		event Action<T> INotifier<T>.Return
		{
			add { this._Return += value; }
			remove { this._Return -= value; }
		}

		T[] INotifier<T>.Returns
		{
			get { return this._RemoveNoRefenceReturns(); }
		}

		IGhost IProvider.Ready(Guid id)
		{
			var entity = (from e in this._Waits where (e as IGhost).GetID() == id select e).FirstOrDefault();
			this._Waits.Remove(entity);
			if (entity != null)
			{
				return this._Add(entity, entity as IGhost);
			}

			return null;
		}

		void IProvider.Add(IGhost entity)
		{
			this._Waits.Add(entity as T);
		}

		void IProvider.Remove(Guid id)
		{
			this._RemoveNoRefenceReturns();

			this._RemoveEntitys(id);

			this._RemoveWaits(id);

			this._RemoveReturns(id);
		}

		IGhost[] IProvider.Ghosts
		{
			get
			{
				var all = this._Entitys.Concat(this._Waits).Concat(from r in this._Returns where r.IsAlive select r.Target as T);
				return (from entity in all select (IGhost)entity).ToArray();
			}
		}

		void IProvider.ClearGhosts()
		{
			this._RemoveNoRefenceReturns();

			if (this._Unsupply != null)
			{
				foreach (var e in this._Entitys)
				{
					this._Unsupply.Invoke(e);
				}
			}

			this._Entitys.Clear();
			this._Waits.Clear();
			this._Returns.Clear();
		}

		private IGhost _Add(T entity, IGhost ghost)
		{
			if (ghost.IsReturnType() == false)
			{
				lock (this._Entitys)
					this._Entitys.Add(entity);
				if (this._Supply != null)
				{
					this._Supply.Invoke(entity);
				}
			}
			else
			{
				this._Returns.Add(new WeakReference(entity));
				if (this._Return != null)
				{
					this._Return(entity);
				}
			}

			return ghost;
		}

		private void _RemoveReturns(Guid id)
		{
			var entity = (from weakRef in this._Returns
			              let e = weakRef.Target as IGhost
			              where weakRef.IsAlive && e.GetID() == id
			              select weakRef).SingleOrDefault();

			if (entity != null)
			{
				this._Returns.Remove(entity);
			}
		}

		private void _RemoveWaits(Guid id)
		{
			var waitentity = (from e in this._Waits where (e as IGhost).GetID() == id select e).FirstOrDefault();
			if (waitentity != null)
			{
				this._Waits.Remove(waitentity);
			}
		}

		private void _RemoveEntitys(Guid id)
		{
			lock (this._Entitys)
			{
				var entity = (from e in this._Entitys where (e as IGhost).GetID() == id select e).FirstOrDefault();
				if (entity != null && this._Unsupply != null)
				{
					this._Unsupply.Invoke(entity);
				}

				this._Entitys.Remove(entity);
			}
		}

		private T[] _RemoveNoRefenceReturns()
		{
			var alives = (from w in this._Returns where w.IsAlive select w.Target as T).ToArray();
			this._Returns.RemoveAll(w => w.IsAlive == false);
			return alives;
		}
	}
}