using Regulus.Memorys;
using Regulus.Remote.Extensions;
using Regulus.Remote.Packages;
using Regulus.Remote.ProviderHelper;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Regulus.Remote
{
    public class GhostProviderQueryer : ClientExchangeable
    {
        private readonly PingHandler _PingHandler;
        private readonly GhostsReturnValueHandler _ReturnValueHandler;
        private readonly GhostsOwner _GhostsOwner;
        private readonly GhostsHandler _GhostManager;
        private readonly GhostsResponer _GhostsResponser;        
        private readonly ClientExchangeable[] ClientExchangeables;

        public bool Active => _GhostsResponser.Active;
        public float Ping => _PingHandler.PingTime;

        

        public GhostProviderQueryer(
            IProtocol protocol,
            ISerializable serializer,
            IInternalSerializable internalSerializer,
            GhostsOwner ghosts_owner)
        {
            _PingHandler = new PingHandler();
            
            _ReturnValueHandler = new GhostsReturnValueHandler(serializer);
            _GhostsOwner = ghosts_owner;
            _GhostManager = new GhostsHandler(protocol, serializer, internalSerializer,  _GhostsOwner, _ReturnValueHandler);
            
            _GhostsResponser = new GhostsResponer(internalSerializer, _GhostManager, _ReturnValueHandler, _PingHandler, _GhostsOwner,  protocol);

            _ReturnValueHandler.ErrorMethodEvent += (method, message) => ErrorMethodEvent?.Invoke(method, message);

            ClientExchangeables = new ClientExchangeable[]
            {
                _PingHandler,                
                _GhostManager,                
            };

            
        }

        event Action<ClientToServerOpCode, Memorys.Buffer> _ResponseEvent;
        event Action<ClientToServerOpCode, Memorys.Buffer> Exchangeable<ServerToClientOpCode, ClientToServerOpCode>.ResponseEvent
        {
            add
            {
                _ResponseEvent += value;
            }

            remove
            {
                _ResponseEvent -= value;
            }
        }

        public event Action<ClientToServerOpCode, Memorys.Buffer> ClientToServerEvent;
        

       

        public void Start()
        {
            foreach (var exchangeable in ClientExchangeables)
            {
                exchangeable.ResponseEvent += _ResponseEvent;
            }

        }

        public void Stop()
        {
            foreach (var exchangeable in ClientExchangeables)
            {
                exchangeable.ResponseEvent -= _ResponseEvent;
            }
            _GhostManager.ClearGhosts();
        }

        public INotifier<T> QueryProvider<T>()
        {
            return _GhostsOwner.QueryProvider<T>();
        }

        void Exchangeable<ServerToClientOpCode, ClientToServerOpCode>.Request(ServerToClientOpCode code, Memorys.Buffer args)
        {
            foreach (var exchangeable in ClientExchangeables)
            {
                exchangeable.Request(code, args);
            }
            _GhostsResponser.OnResponse(code, args);
        }

        public event Action<string, string> ErrorMethodEvent;
    }
    
    delegate System.Action<object> GetObjectAccesserMethod(IObjectAccessible accessible);
   
}
