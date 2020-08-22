


using Regulus.Network;
using Regulus.Remote.Ghost;
using System;
using System.Collections.Generic;

namespace Regulus.Remote.Standalone
{
    public class Service : IService
    {

        
        readonly Regulus.Remote.Soul.IService _Service;
        readonly List<Regulus.Remote.Ghost.IAgent> _Agents;
        readonly Dictionary<IAgent, IStreamable> _Streams;
        readonly IDisposable _ServiceDisposable;
        public Service(IBinderProvider entry, IProtocol protocol)
        {        
            _Service = new Regulus.Remote.Soul.Service(entry, protocol);
            _Agents = new List<Ghost.IAgent>();
            _Streams = new Dictionary<IAgent, IStreamable>();
            _ServiceDisposable = _Service;
        }

        public void Join(IAgent agent,object state)
        {
            
            Stream stream = new Stream();
            agent.Start(new ReverseStream(stream));
            _Service.Join(stream, state);
            _Streams.Add(agent, stream);
            _Agents.Add(agent);            
        }

        public void Leave(IAgent queryable)
        {
            IAgent remove = null;
            foreach (IAgent agent in _Agents)
            {
                if (agent != queryable)
                    continue;

                _Service.Leave(_Streams[agent]);
                agent.Stop();
                remove = agent;
            }

            _Agents.Remove(remove);
        }

        
        void IDisposable.Dispose()
        {
            _ServiceDisposable.Dispose();
        }
    }
}
