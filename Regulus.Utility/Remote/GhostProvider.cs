using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote
{

	public class TProvider<T> : INotifier<T>, IProvider
		
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
				_Supply += value;

				lock(_Entitys)
				{
					foreach(var e in _Entitys.ToArray())
					{
						value(e);
					}
				}
			}

			remove { _Supply -= value; }
		}

		event Action<T> INotifier<T>.Unsupply
		{
			add { _Unsupply += value; }
			remove { _Unsupply -= value; }
		}

		T[] INotifier<T>.Ghosts
		{
			get
			{
				lock(_Entitys)
					return _Entitys.ToArray();
			}
		}

		event Action<T> INotifier<T>.Return
		{
			add { _Return += value; }
			remove { _Return -= value; }
		}

		T[] INotifier<T>.Returns
		{
			get { return _RemoveNoRefenceReturns(); }
		}

		IGhost IProvider.Ready(long id)
		{
			var entity = (from e in _Waits where (e as IGhost).GetID() == id select e).FirstOrDefault();
			_Waits.Remove(entity);
			if(entity != null)
			{
				return _Add(entity, entity as IGhost);
			}

			return null;
		}

		void IProvider.Add(IGhost entity)
		{
			_Waits.Add((T)entity );
		}

		void IProvider.Remove(long id)
		{
			_RemoveNoRefenceReturns();

			_RemoveEntitys(id);

			_RemoveWaits(id);

			_RemoveReturns(id);
		}

		IGhost[] IProvider.Ghosts
		{
			get
			{
				var all = _Entitys.Concat(_Waits).Concat(from r in _Returns where r.IsAlive select (T)r.Target  );
				return (from entity in all select (IGhost)entity).ToArray();
			}
		}

		void IProvider.ClearGhosts()
		{
			_RemoveNoRefenceReturns();

			if(_Unsupply != null)
			{
				foreach(var e in _Entitys)
				{
					_Unsupply.Invoke(e);
				}
			}

			_Entitys.Clear();
			_Waits.Clear();
			_Returns.Clear();
		}

		private IGhost _Add(T entity, IGhost ghost)
		{
			if(ghost.IsReturnType() == false)
			{
				lock(_Entitys)
					_Entitys.Add(entity);
				if(_Supply != null)
				{
					_Supply.Invoke(entity);
				}
			}
			else
			{
				_Returns.Add(new WeakReference(entity));
				if(_Return != null)
				{
					_Return(entity);
				}
			}

			return ghost;
		}

		private void _RemoveReturns(long id)
		{
			var entity = (from weakRef in _Returns
			              let e = weakRef.Target as IGhost
			              where weakRef.IsAlive && e.GetID() == id
			              select weakRef).SingleOrDefault();

			if(entity != null)
			{
				_Returns.Remove(entity);
			}
		}

		private void _RemoveWaits(long id)
		{
			var waitentity = (from e in _Waits where (e as IGhost).GetID() == id select e).FirstOrDefault();
			if(waitentity != null)
			{
				_Waits.Remove(waitentity);
			}
		}

		private void _RemoveEntitys(long id)
		{
			lock(_Entitys)
			{
				var entity = (from e in _Entitys where (e as IGhost).GetID() == id select e).FirstOrDefault();
				if(entity != null && _Unsupply != null)
				{
					_Unsupply.Invoke(entity);
				}

				_Entitys.Remove(entity);
			}
		}

		private T[] _RemoveNoRefenceReturns()
		{
			var alives = (from w in _Returns where w.IsAlive select (T)w.Target  ).ToArray();
			_Returns.RemoveAll(w => w.IsAlive == false);
			return alives;
		}
	}
}
