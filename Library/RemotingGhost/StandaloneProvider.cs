using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting.Standalone
{
	public class Provider<T> : Samebest.Remoting.Ghost.IProviderNotice<T>
	{
		System.Collections.Generic.List<T> _Entitys;
		public Provider()
		{
			_Entitys = new List<T>();
		}
		public void Add(T entity)
		{
			_Entitys.Add(entity);
			if (_SupplyEvent != null)
				_SupplyEvent.Invoke(entity);
		}

		public void Remove(T entity)
		{
			if (_Entitys.Remove(entity) && _UnsupplyEvent != null)
				_UnsupplyEvent.Invoke(entity);
		}


		event Action<T> _SupplyEvent;
		event Action<T> Samebest.Remoting.Ghost.IProviderNotice<T>.Supply
		{
			add
			{
				_SupplyEvent += value;
				foreach (var ent in _Entitys)
				{
					value.Invoke(ent);
				}
			}
			remove { _SupplyEvent -= value; }
		}

		event Action<T> _UnsupplyEvent;
		event Action<T> Samebest.Remoting.Ghost.IProviderNotice<T>.Unsupply
		{
			add { _UnsupplyEvent += value; }
			remove { _UnsupplyEvent -= value; }
		}

		T[] Samebest.Remoting.Ghost.IProviderNotice<T>.Ghosts
		{
			get { return _Entitys.ToArray(); }
		}
	}
}