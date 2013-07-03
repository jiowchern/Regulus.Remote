using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost
{
	

	

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
