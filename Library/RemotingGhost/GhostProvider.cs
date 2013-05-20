using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting.Ghost
{
	public interface IProviderNotice<T>
	{		
		event Action<T> Supply;
		event Action<T> Unsupply;

		T[] Ghosts { get; }	
	}

	public interface IProvider
	{
		IGhost[] Ghosts { get; }	
		void Add(IGhost entiry);
		void Remove(Guid id);
	}

	public class TProvider<T> : IProviderNotice<T>, IProvider where T : class
	{		
		List<T>	_Entitys = new List<T>();

		event Action<T> _Supply;
		event Action<T> IProviderNotice<T>.Supply
		{
			add { _Supply += value; }
			remove { _Supply -= value; }
		}

		event Action<T> _Unsupply;
		event Action<T> IProviderNotice<T>.Unsupply
		{
			add { _Unsupply += value; }
			remove { _Unsupply -= value; }
		}

		void IProvider.Add(IGhost entity)
		{
			_Entitys.Add(entity as T);
			if(_Supply != null)
				_Supply.Invoke(entity as T);
		}

		void IProvider.Remove(Guid id)
		{
			var entity = (from e in _Entitys where (e as IGhost).GetID() == id select e).FirstOrDefault();
			if (entity != null && _Unsupply != null)
			{
				_Unsupply.Invoke(entity);
			}

			_Entitys.Remove(entity);
		}

		IGhost[] IProvider.Ghosts
		{
			get { return (from entity in _Entitys select (IGhost)entity).ToArray() ; }
		}


		T[] IProviderNotice<T>.Ghosts
		{
			get { return _Entitys.ToArray(); }
		}
	}

	

	
}
