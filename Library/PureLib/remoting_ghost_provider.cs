using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost
{
	public interface IProviderNotice<T>
	{
		event Action<T> Supply;
		event Action<T> Unsupply;
        event Action<T> Return;

		T[] Ghosts { get; }
        T[] Returns { get; }
	}

	public interface IProvider
	{
		IGhost[] Ghosts { get; }
		void Add(IGhost entiry);
		void Remove(Guid id);
        IGhost Ready(Guid id);
	}


	public class TProvider<T> : IProviderNotice<T>, IProvider
        where T : class 
        
	{
		List<T> _Entitys = new List<T>();
        List<WeakReference> _Returns = new List<WeakReference>();

		event Action<T> _Supply;
		event Action<T> IProviderNotice<T>.Supply
		{
			add
			{
                
				_Supply += value;

                lock (_Entitys)
                {
                    foreach (var e in _Entitys)
                    {
                        value(e);
                    }
                }
				
			}
			remove { _Supply -= value; }
		}

		event Action<T> _Unsupply;
		event Action<T> IProviderNotice<T>.Unsupply
		{
			add { _Unsupply += value; }
			remove { _Unsupply -= value; }
		}

		List<T> _Waits = new List<T>();
        IGhost IProvider.Ready(Guid id)
		{
			var entity = (from e in _Waits where (e as IGhost).GetID() == id select e).FirstOrDefault();
			_Waits.Remove(entity);
			if (entity != null)
                return _Add(entity, entity as IGhost);
            return null;
		}

        IGhost _Add(T entity, IGhost ghost)
		{
            if (ghost.IsReturnType() == false)
            {
                lock (_Entitys)
                    _Entitys.Add(entity);
                if (_Supply != null)
                    _Supply.Invoke(entity);
            }
			else
            {
                _Returns.Add(new WeakReference(entity));
                if (_Return != null)
                    _Return(entity);
            }

            return ghost;
		}
		void IProvider.Add(IGhost entity)
		{
			_Waits.Add(entity as T);
            
		}

		void IProvider.Remove(Guid id)
		{
            _RemoveNoRefenceReturns();

            _RemoveEntitys(id);


            _RemoveWaits(id);

            _RemoveReturns(id);

		}

        private void _RemoveReturns(Guid id)
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

        private void _RemoveWaits(Guid id)
        {
            var waitentity = (from e in _Waits where (e as IGhost).GetID() == id select e).FirstOrDefault();
            if (waitentity != null)
                _Waits.Remove(waitentity);
        }

        private void _RemoveEntitys(Guid id)
        {
            lock (_Entitys)
            {
                var entity = (from e in _Entitys where (e as IGhost).GetID() == id select e).FirstOrDefault();
                if (entity != null && _Unsupply != null)
                {
                    _Unsupply.Invoke(entity);
                }

                _Entitys.Remove(entity);
            }
            
        }

		IGhost[] IProvider.Ghosts
		{

			get
			{                
                var all = _Entitys.Concat(_Waits).Concat( from r in _Returns where r.IsAlive select r.Target as T);
				return (from entity in all select (IGhost)entity).ToArray();
			}
		}


		T[] IProviderNotice<T>.Ghosts
		{
			get
			{
                lock (_Entitys)
				    return _Entitys.ToArray();
			}
		}


        event Action<T> _Return;
        event Action<T> IProviderNotice<T>.Return
        {
            add { _Return += value; }
            remove { _Return -= value; }
        }


        T[] IProviderNotice<T>.Returns
        {
            get 
            {
                return _RemoveNoRefenceReturns();
            }
        }

        private T[] _RemoveNoRefenceReturns()
        {
            T[] alives = (from w in _Returns where w.IsAlive select w.Target as T).ToArray();
            _Returns.RemoveAll(w => w.IsAlive == false);
            return alives;
        }
    }
}
