using Regulus.Memorys;
using Regulus.Network;
using Regulus.Remote.ProviderHelper;
using Regulus.Utility;
using System;
using System.IO;



namespace Regulus.Remote.Ghost
{
    public class Agent : IAgent
    {

        private readonly GhostProviderQueryer _GhostProvider;        
        private readonly GhostsOwner _GhostsOwner;
        private readonly IPool _Pool;
        private readonly IInternalSerializable _InternalSerializer;

        private System.Action _GhostSerializerUpdater;
        private System.Action _GhostSerializerStop;
        private float _Ping
        {
            get { return _GhostProvider.Ping; }
        }

        public Agent(IProtocol protocol, ISerializable serializable , IInternalSerializable internal_serializable , Regulus.Memorys.IPool pool)
        {
            _InternalSerializer = internal_serializable;
            _Pool = pool;
            _GhostsOwner = new GhostsOwner(protocol);
                        
            _GhostProvider = new GhostProviderQueryer(protocol, serializable, internal_serializable, _GhostsOwner);
            _GhostSerializerUpdater = () => { };
            _GhostSerializerStop = () => { };

            Singleton<Log>.Instance.WriteInfo("Agent Launch.");
            _GhostProvider.ErrorMethodEvent += _ErrorMethodEvent;
        }

        void IAgent.Update()
        {
            _GhostSerializerUpdater();            

        }

        public void Enable(IStreamable streamable)
        {
            Disable();
            var ghostSerializer = new GhostSerializer(new PackageReader(streamable, _Pool), new PackageSender(streamable, _Pool), _InternalSerializer);
            ServerExchangeable serverExchangeable = ghostSerializer;
            ClientExchangeable clientExchangeable = _GhostProvider;
            serverExchangeable.ResponseEvent += clientExchangeable.Request;
            clientExchangeable.ResponseEvent += serverExchangeable.Request;

            _GhostSerializerStop =
            () =>
            {
                serverExchangeable.ResponseEvent -= clientExchangeable.Request;
                clientExchangeable.ResponseEvent -= serverExchangeable.Request;
            };

            _GhostProvider.Start();
            ghostSerializer.Start();


            _GhostSerializerUpdater = ghostSerializer.Update;

            
        }
        public void Disable()
        {
            
            _GhostSerializerUpdater = () => { };

            _GhostSerializerStop();
            _GhostSerializerStop = () => { };
            _GhostsOwner.ClearProviders();
            _GhostProvider.Stop();

            


        }
        INotifier<T> INotifierQueryable.QueryNotifier<T>()
        {
            return _GhostsOwner.QueryProvider<T>();
        }

        /*void IDisposable.Dispose()
        {
            
            Disable();

            _GhostProvider.ErrorMethodEvent -= _ErrorMethodEvent;

            Singleton<Log>.Instance.WriteInfo("Agent Shutdown.");
        }*/

        float IAgent.Ping
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
