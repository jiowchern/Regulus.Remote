


using Regulus.Network;
using Regulus.Remote.Ghost;
using System;
using System.Collections.Generic;

namespace Regulus.Remote.Standalone
{
    public class Service : IDisposable
    {

        private readonly IProtocol _Protocol;
        readonly Regulus.Remote.Soul.Service _Service;
        readonly List<Regulus.Remote.Ghost.IAgent> _Agents;
        readonly Dictionary<IAgent, IStreamable> _Streams;
        readonly IDisposable _ServiceDisposable;
        public Service(IBinderProvider entry, IProtocol protocol)
        {
            this._Protocol = protocol;
            _Service = new Regulus.Remote.Soul.Service(entry, protocol);
            _Agents = new List<Ghost.IAgent>();
            _Streams = new Dictionary<IAgent, IStreamable>();
            _ServiceDisposable = _Service;
        }

        public INotifierQueryable CreateNotifierQueryer()
        {
            IAgent agent = new Ghost.Agent(_Protocol) as IAgent;
            Stream stream = new Stream();
            agent.Start(new ReverseStream(stream));
            _Service.Join(stream);
            _Streams.Add(agent, stream);
            _Agents.Add(agent);
            return agent;
        }

        public void DestroyNotifierQueryer(INotifierQueryable queryable)
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

        public void Update()
        {
            foreach (IAgent agent in _Agents)
            {
                agent.Update();
            }
        }

        void IDisposable.Dispose()
        {
            _ServiceDisposable.Dispose();
        }
    }
}
