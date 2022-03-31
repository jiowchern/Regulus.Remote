using Regulus.Network;
using Regulus.Utility;
using System;



namespace Regulus.Remote.Ghost
{
    public class Agent : IAgent
    {

        private readonly GhostProviderQueryer _GhostProvider;
        private readonly GhostSerializer _GhostSerializer;
        private readonly IInternalSerializable _InternalSerializer;
        readonly IStreamable _Stream;
        private long _Ping
        {
            get { return _GhostProvider.Ping; }
        }

        public Agent(IStreamable stream,IProtocol protocol, ISerializable serializable , IInternalSerializable internal_serializable             )
        {
            _Stream = stream;
            _InternalSerializer = internal_serializable;

            GhostSerializer ghostSerializer = new GhostSerializer(new PackageReader<Regulus.Remote.Packages.ResponsePackage>(_InternalSerializer), new PackageWriter<Regulus.Remote.Packages.RequestPackage>(_InternalSerializer));
            _GhostProvider = new GhostProviderQueryer(protocol, serializable, internal_serializable, ghostSerializer);            
            _GhostSerializer = ghostSerializer;


            Singleton<Log>.Instance.WriteInfo("Agent Launch.");
            _GhostProvider.ErrorMethodEvent += _ErrorMethodEvent;

            _GhostProvider.Start();
            _GhostSerializer.Start(stream);
        }

        void IAgent.Update()
        {
            _GhostSerializer.Update();
        }

        

   

        INotifier<T> INotifierQueryable.QueryNotifier<T>()
        {
            return _GhostProvider.QueryProvider<T>();
        }

        void IDisposable.Dispose()
        {
            _GhostSerializer.Stop();
            _GhostProvider.Stop();

            _GhostProvider.ErrorMethodEvent -= _ErrorMethodEvent;

            Singleton<Log>.Instance.WriteInfo("Agent Shutdown.");
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
