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
        void Ready(Guid id);
	}

	public class TProvider<T> : IProviderNotice<T>, IProvider where T : class
	{		
		List<T>	_Entitys = new List<T>();

		event Action<T> _Supply;
		event Action<T> IProviderNotice<T>.Supply
		{
			add 
            {
                _Supply += value; 
                foreach (var e in _Entitys)
                {
                    value(e);
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
        void IProvider.Ready(Guid id)
        {
            var entity = (from e in _Waits where (e as IGhost).GetID() == id select e).FirstOrDefault();
            _Waits.Remove(entity);
            if (entity != null)
                _Add(entity);
        }

        void _Add(T entity)
        {
            _Entitys.Add(entity);
            if (_Supply != null)
                _Supply.Invoke(entity as T);
        }
		void IProvider.Add(IGhost entity)
		{
            _Waits.Add(entity as T);
		}

		void IProvider.Remove(Guid id)
		{
			var entity = (from e in _Entitys where (e as IGhost).GetID() == id select e).FirstOrDefault();
			if (entity != null && _Unsupply != null)
			{
				_Unsupply.Invoke(entity);
			}

			_Entitys.Remove(entity);


            var waitentity = (from e in _Waits where (e as IGhost).GetID() == id select e).FirstOrDefault();
            if (waitentity != null)
                _Waits.Remove(waitentity);
		}

		IGhost[] IProvider.Ghosts
		{
            
			get 
            {
                var all = _Entitys.Concat(_Waits);
                return (from entity in all select (IGhost)entity).ToArray(); 
            }
		}


		T[] IProviderNotice<T>.Ghosts
		{
			get 
            {
                return _Entitys.ToArray(); 
            }
		}
	}

    public class Provider<T> : IProviderNotice<T> 
    {
        List<Agent> _Agents = new List<Agent>();

        List<Action<T>> _SupplyActions = new List<Action<T>>();
        List<Action<T>> _UnsupplyActions = new List<Action<T>>();
        
        public void Add(Agent agent)
        {
            _Agents.Add(agent);
            foreach(var action in _SupplyActions)
            {
                agent.QueryProvider<T>().Supply += action;
            }
            foreach (var action in _UnsupplyActions)
            {
                agent.QueryProvider<T>().Unsupply += action;
            }
        }

        public void Remove(Agent agent)
        {
            _Agents.Remove(agent);
            foreach (var action in _SupplyActions)
            {
                agent.QueryProvider<T>().Supply -= action;
            }
            foreach (var action in _UnsupplyActions)
            {
                agent.QueryProvider<T>().Unsupply -= action;
            }
            if (_Agents.Count == 0)
            {
                _SupplyActions.Clear();
                _UnsupplyActions.Clear();
            }
        }

        event Action<T> IProviderNotice<T>.Supply
        {
            add 
            {
                foreach (var agent in _Agents)
                {
                    var provider = agent.QueryProvider<T>();
                    provider.Supply += value;                    
                }
                _SupplyActions.Add(value);
            }
            remove 
            {
                foreach (var agent in _Agents)
                {
                    var provider = agent.QueryProvider<T>();
                    provider.Supply -= value;                    
                }
                _SupplyActions.Remove(value);
            }
        }

        event Action<T> IProviderNotice<T>.Unsupply
        {
            add 
            {
                foreach (var agent in _Agents)
                {
                    var provider = agent.QueryProvider<T>();
                    provider.Unsupply += value;                    
                }
                _UnsupplyActions.Add(value);
            }
            remove 
            {
                foreach (var agent in _Agents)
                {
                    var provider = agent.QueryProvider<T>();
                    provider.Unsupply -= value;                    
                }
                _UnsupplyActions.Remove(value);
            }
        }

        T[] IProviderNotice<T>.Ghosts
        {
            get 
            {
                List<T> ghosts = new List<T>();
                foreach (var agent in _Agents)
                {
                    var provider = agent.QueryProvider<T>();
                    ghosts.AddRange(provider.Ghosts);
                }
                return ghosts.ToArray();
            }
        }
    }
	
}
