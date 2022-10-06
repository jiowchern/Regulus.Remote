


using Regulus.Network;
using Regulus.Remote.Ghost;
using Regulus.Remote.Soul;
using System;
using System.Collections.Generic;

namespace Regulus.Remote.Standalone
{
    public class Service : Soul.IService , Soul.IListenable
    {

        
        readonly Regulus.Remote.Soul.IService _Service;
        readonly List<Regulus.Remote.Ghost.IAgent> _Agents;
        readonly Dictionary<IAgent, IStreamable> _Streams;
        readonly IDisposable _ServiceDisposable;
        
        internal readonly IProtocol Protocol;
        internal readonly ISerializable Serializer;

        readonly NotifiableCollection<IStreamable> _NotifiableCollection;
        public Service(IBinderProvider entry, IProtocol protocol , ISerializable serializable, Regulus.Remote.IInternalSerializable internal_serializable)
        {
            _NotifiableCollection = new NotifiableCollection<IStreamable>();
            Protocol = protocol;
            Serializer = serializable;
            var service = new Regulus.Remote.Soul.AsyncService(new SyncService(entry , new UserProvider(protocol, serializable, this, internal_serializable)) );
            _Service = service;
        
            _Agents = new List<Ghost.IAgent>();
            _Streams = new Dictionary<IAgent, IStreamable>();
            _ServiceDisposable = _Service;
        }


        event Action<IStreamable> Soul.IListenable.StreamableEnterEvent
        {
            add
            {
                _NotifiableCollection.Notifier.Supply += value;
            }

            remove
            {
                _NotifiableCollection.Notifier.Supply -= value;
            }
        }

        event Action<IStreamable> Soul.IListenable.StreamableLeaveEvent
        {
            add
            {
                _NotifiableCollection.Notifier.Unsupply += value;
            }

            remove
            {
                _NotifiableCollection.Notifier.Unsupply -= value;
            }
        }


     

        public Ghost.IAgent Create()
        {
            var stream = new Stream();
            var agent = new Regulus.Remote.Ghost.Agent(stream, this.Protocol, this.Serializer, new Regulus.Remote.InternalSerializer());
            var revStream = new ReverseStream(stream);

            _NotifiableCollection.Items.Add(revStream);
            _Streams.Add(agent, revStream);
            _Agents.Add(agent);

            
            return agent;
        }
        

        public void Destroy(IAgent queryable)
        {
            var agents = new System.Collections.Generic.List<IAgent>();
            foreach (IAgent agent in _Agents)
            {
                if (agent != queryable)
                    continue;
                
                _NotifiableCollection.Items.Remove(_Streams[agent]);
                _Streams.Remove(agent);
                agent.Dispose();
                agents.Add(agent);
            }
            foreach (IAgent agent in agents)
            {
                _Agents.Remove(agent);
            }
            
        }

        
        public void Dispose()
        {
            _ServiceDisposable.Dispose();
        }
    }
}
