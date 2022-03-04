using Regulus.Network;
using Regulus.Serialization;
using Regulus.Utility;
using System;



namespace Regulus.Remote.Ghost
{
    public class Agent : IAgent
    {

        private readonly GhostProvider _GhostProvider;
        private readonly GhostSerializer _GhostSerializer;

        private long _Ping
        {
            get { return _GhostProvider.Ping; }
        }

        public Agent(IProtocol protocol, ISerializable serializable)
        {
            GhostSerializer ghostSerializer = new GhostSerializer(new PackageReader<ResponsePackage>(serializable) , new PackageWriter<RequestPackage>(serializable));

            _GhostProvider = new GhostProvider(protocol, serializable, ghostSerializer);

            _GhostSerializer = ghostSerializer;
        }

        void IAgent.Update()
        {
            _GhostSerializer.Update();
        }

        void IAgent.Start(IStreamable peer)
        {
            Singleton<Log>.Instance.WriteInfo("Agent Launch.");
            _GhostProvider.ErrorMethodEvent += _ErrorMethodEvent;
            
            _GhostProvider.Start();
            _GhostSerializer.Start(peer);
            
        }

        void IAgent.Stop()
        {
            _GhostSerializer.Stop();
            _GhostProvider.Stop();            
            
            _GhostProvider.ErrorMethodEvent -= _ErrorMethodEvent;

            Singleton<Log>.Instance.WriteInfo("Agent Shutdown.");
        }

        INotifier<T> INotifierQueryable.QueryNotifier<T>()
        {
            return _GhostProvider.QueryProvider<T>();
        }

        long IAgent.Ping
        {
            get { return _Ping; }
        }

        bool IAgent.Active => _GhostProvider.Active;

        private event Action<string, string> _ErrorMethodEvent;

        event Action<string, string> IAgent.ErrorMethodEvent
        {
            add { this._ErrorMethodEvent += value; }
            remove { this._ErrorMethodEvent -= value; }
        }

        


    }
}
