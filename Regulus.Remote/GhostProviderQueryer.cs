using Regulus.Memorys;
using Regulus.Remote.Extensions;
using Regulus.Remote.Packages;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Regulus.Remote
{
    delegate System.Action<object> GetObjectAccesserMethod(IObjectAccessible accessible);
    public class GhostProviderQueryer
    {
        private readonly AutoRelease<long ,IGhost> _AutoRelease;

        private readonly Dictionary<Type, IProvider> _Providers;

        private readonly Dictionary<long, GhostResponseHandler> _GhostHandlers;

        private readonly ReturnValueQueue _ReturnValueQueue;

        

        private readonly IOpCodeExchangeable _Exchanger;

        private readonly InterfaceProvider _InterfaceProvider;
        private readonly ISerializable _Serializer;        
       
        private readonly IProtocol _Protocol;

        bool _Active;
        public bool Active => _Active;

        public float Ping => _Ping.GetSeconds();

        readonly IInternalSerializable _InternalSerializer;
        readonly Ping _Ping;

        public GhostProviderQueryer(IProtocol protocol, ISerializable serializable, IInternalSerializable internal_serializable, IOpCodeExchangeable exchanger)
        {
            _Ping = new Ping(1f);
            _Ping.TriggerEvent += _SendPing;
            _InternalSerializer = internal_serializable;
            _Active = false;
            _Exchanger = exchanger;

            _ReturnValueQueue = new ReturnValueQueue();
            _Protocol = protocol;
            _InterfaceProvider = _Protocol.GetInterfaceProvider();
            _Serializer = serializable;
            _Providers = new Dictionary<Type, IProvider>();
            _GhostHandlers = new Dictionary<long, GhostResponseHandler>();
            _AutoRelease = new AutoRelease<long,IGhost>();
        }

        private void _SendPing()
        {
            _Exchanger.Request(ClientToServerOpCode.Ping, new byte[0].AsBuffer());
        }

        public void Start()
        {
            _Exchanger.ResponseEvent += _OnResponse;
            
        }
        public void Stop()
        {
            _Exchanger.ResponseEvent -= _OnResponse;
            
            lock (_Providers)
            {
                foreach (KeyValuePair<Type, IProvider> providerPair in _Providers)
                {
                    providerPair.Value.ClearGhosts();
                }
            }
            lock(_GhostHandlers)
                _GhostHandlers.Clear();

            
        }

        

        protected void _OnResponse(ServerToClientOpCode code, Regulus.Memorys.Buffer args)
        {
            _UpdateAutoRelease();
            if (code == ServerToClientOpCode.Ping)
            {
                _Ping.Update();
                
            }
            else if (code == ServerToClientOpCode.SetProperty)
            {

                Regulus.Remote.Packages.PackageSetProperty data = (Regulus.Remote.Packages.PackageSetProperty)_InternalSerializer.Deserialize(args);
                _UpdateSetProperty(data.EntityId, data.Property, data.Value);
            }

            else if (code == ServerToClientOpCode.InvokeEvent)
            {
                Regulus.Remote.Packages.PackageInvokeEvent data = (Regulus.Remote.Packages.PackageInvokeEvent)_InternalSerializer.Deserialize(args);
                _InvokeEvent(data.EntityId, data.Event, data.HandlerId, data.EventParams);
            }
            else if (code == ServerToClientOpCode.ErrorMethod)
            {
                Regulus.Remote.Packages.PackageErrorMethod data = (Regulus.Remote.Packages.PackageErrorMethod)_InternalSerializer.Deserialize(args);

                _ErrorReturnValue(data.ReturnTarget, data.Method, data.Message);
            }
            else if (code == ServerToClientOpCode.ReturnValue)
            {
                Regulus.Remote.Packages.PackageReturnValue data = (Regulus.Remote.Packages.PackageReturnValue)_InternalSerializer.Deserialize(args);
                _SetReturnValue(data.ReturnTarget, data.ReturnValue);
            }
            else if (code == ServerToClientOpCode.LoadSoulCompile)
            {

                Regulus.Remote.Packages.PackageLoadSoulCompile data = (Regulus.Remote.Packages.PackageLoadSoulCompile)_InternalSerializer.Deserialize(args);
                _LoadSoulCompile(data.TypeId, data.EntityId, data.ReturnId);

            }
            else if (code == ServerToClientOpCode.LoadSoul)
            {
                Regulus.Remote.Packages.PackageLoadSoul data = (Regulus.Remote.Packages.PackageLoadSoul)_InternalSerializer.Deserialize(args);
                _LoadSoul(data.TypeId, data.EntityId, data.ReturnType);
            }
            else if (code == ServerToClientOpCode.UnloadSoul)
            {
                Regulus.Remote.Packages.PackageUnloadSoul data = (Regulus.Remote.Packages.PackageUnloadSoul)_InternalSerializer.Deserialize(args);

                _UnloadSoul(data.EntityId);
            }
            else if (code == ServerToClientOpCode.AddPropertySoul)
            {
                Regulus.Remote.Packages.PackagePropertySoul data = (Regulus.Remote.Packages.PackagePropertySoul)_InternalSerializer.Deserialize(args);

                _AddPropertySoul(data);
            }
            else if (code == ServerToClientOpCode.RemovePropertySoul)
            {
                Regulus.Remote.Packages.PackagePropertySoul data = (Regulus.Remote.Packages.PackagePropertySoul)_InternalSerializer.Deserialize(args);

                _RemovePropertySoul(data);
            }
            else if (code == ServerToClientOpCode.ProtocolSubmit)
            {
                Regulus.Remote.Packages.PackageProtocolSubmit data = (Regulus.Remote.Packages.PackageProtocolSubmit)_InternalSerializer.Deserialize(args);

                _ProtocolSubmit(data);
            }
            else
            {

            }
        }

        private void _UpdateAutoRelease()
        {
            foreach (var id in _AutoRelease.NoExist())
            {
                var pkg = new Regulus.Remote.Packages.PackageRelease();
                pkg.EntityId = id;
                _Exchanger.Request(ClientToServerOpCode.Release, _InternalSerializer.Serialize(pkg));

                _UnloadSoul(id);
            }
        }

        private void _ProtocolSubmit(Regulus.Remote.Packages.PackageProtocolSubmit data)
        {
            _Active = _Comparison(_Protocol.VerificationCode, data.VerificationCode);

        }

        private bool _Comparison(byte[] code1, byte[] code2)
        {
            return new Regulus.Utility.Comparer<byte>(code1, code2, (arg1, arg2) => arg1 == arg2).Same;
        }

        private void _ErrorReturnValue(long return_target, string method, string message)
        {
            _ReturnValueQueue.PopReturnValue(return_target);

            if (ErrorMethodEvent != null)
            {
                ErrorMethodEvent(method, message);
            }
        }

        private void _SetReturnValue(long returnTarget, byte[] returnValue)
        {
            IValue value = _ReturnValueQueue.PopReturnValue(returnTarget);
            if (value != null)
            {
                object returnInstance = _Serializer.Deserialize(value.GetObjectType() , returnValue.AsBuffer());
                value.SetValue(returnInstance);
            }
        }

        private void _ReturnValue(long return_id, IGhost ghost)
        {
            IValue value = _ReturnValueQueue.PopReturnValue(return_id);
            if (value != null)
            {
                value.SetValue(ghost);
            }
        }

        private void _LoadSoulCompile(int type_id, long entity_id, long return_id )
        {
            
            MemberMap map = _Protocol.GetMemberMap();
            
            Type type = map.GetInterface(type_id);
            
            IProvider provider = _QueryProvider(type);

            var handler = _FindHandler(entity_id);
            if (return_id != 0)
            {
                _ReturnValue(return_id, handler.FindGhost());
            }
            else
            {
                provider.Ready(entity_id);
            }
                
        }

        private void _LoadSoul(int type_id, long id, bool return_type)
        {
            var map = _Protocol.GetMemberMap();
            var type = map.GetInterface(type_id);
            var ghost = _BuildGhost(type, id, return_type);
            
            ghost.CallMethodEvent += new GhostMethodHandler(ghost.GetID(), _ReturnValueQueue, _Protocol, _Serializer, _InternalSerializer, _Exchanger).Run;
            ghost.AddEventEvent += new GhostEventMoveHandler(ghost.GetID(), _Protocol, _InternalSerializer, _Exchanger).Add;
            ghost.RemoveEventEvent += new GhostEventMoveHandler(ghost.GetID(), _Protocol, _InternalSerializer, _Exchanger).Remove;            
            
            lock (_GhostHandlers)
                _GhostHandlers.Add(ghost.GetID(), new GhostResponseHandler(new WeakReference<IGhost>(ghost), _Protocol.GetMemberMap(), _Serializer));

            if (ghost.IsReturnType())
            {
                _AutoRelease.Push(ghost.GetID(), ghost);                
            }
            else
            {
                var provider = _QueryProvider(type);
                provider.Add(ghost);
            }
        }

        private void _UnloadSoul(long id)
        {
            lock(_Providers)
            {
                foreach (var provider in _Providers.Values)
                {
                    provider.Remove(id);
                }
            }
            lock (_GhostHandlers)
                _GhostHandlers.Remove(id);
        }

        private IProvider _QueryProvider(Type type)
        {
            IProvider provider = null;
            lock (_Providers)
            {
                if (_Providers.TryGetValue(type, out provider) == false)
                {
                    provider = _BuildProvider(type);
                    _Providers.Add(type, provider);
                }
            }

            return provider;
        }

        private IProvider _BuildProvider(Type type)
        {
            MemberMap map = _Protocol.GetMemberMap();
            return map.CreateProvider(type);
        }
        public INotifier<T> QueryProvider<T>()
        {
            return _QueryProvider(typeof(T)) as INotifier<T>;
        }
        private void _RemovePropertySoul(Regulus.Remote.Packages.PackagePropertySoul data)
        {
            _PropertySoulAccesser(data, (a) => a.Remove);            
        }
        private void _AddPropertySoul(Regulus.Remote.Packages.PackagePropertySoul data)
        {
            _PropertySoulAccesser(data, (a) => a.Add);
        }

        internal void _PropertySoulAccesser(PackagePropertySoul data, System.Linq.Expressions.Expression<GetObjectAccesserMethod> oper)
        {

            var owner_handler = _FindHandler(data.OwnerId);
            if (owner_handler == null)
                return;

            var entity = _FindHandler(data.EntityId);
            if (entity == null)
                return;

            var entity_ghost = entity.FindGhost();

            oper.Execute().Invoke(owner_handler.GetAccesser(data.PropertyId) , new object[] { entity_ghost.GetInstance() });
        }

        

        

        private void _UpdateSetProperty(long entity_id, int property, byte[] payload)
        {
            var ghost = _FindHandler(entity_id);
            if (ghost == null)
                return;
            ghost.UpdateSetProperty(property, payload);

            Regulus.Remote.Packages.PackageSetPropertyDone pkg = new Regulus.Remote.Packages.PackageSetPropertyDone();
            pkg.EntityId = entity_id;
            pkg.Property = property;
            
            _Exchanger.Request(ClientToServerOpCode.UpdateProperty, _InternalSerializer.Serialize(pkg));
        }

        private void _InvokeEvent(long ghost_id, int event_id, long handler_id, byte[][] event_params)
        {
            var ghost = _FindHandler(ghost_id);
            if (ghost == null)
            {
                return;
            }

            ghost.InvokeEvent(event_id, handler_id, event_params);

        }       

        private GhostResponseHandler _FindHandler(long ghost_id)
        {
            GhostResponseHandler handler = null;
            lock (_GhostHandlers)
            {
                if(_GhostHandlers.ContainsKey(ghost_id))
                {
                    handler = _GhostHandlers[ghost_id];
                    
                    if (handler.IsValid())
                    {
                        return handler;
                    }                                            
                }

            }
            
            return handler;
        }

        

        

        

        private IGhost _BuildGhost(Type ghost_base_type, long id, bool return_type)
        {
            Type ghostType = _QueryGhostType(ghost_base_type);

            ConstructorInfo constructor = ghostType.GetConstructor(new[] { typeof(long), typeof(bool) });
            
            object o = constructor.Invoke(new object[] { id, return_type });

            return (IGhost)o;
        }



        private Type _QueryGhostType(Type ghostBaseType)
        {
            return _InterfaceProvider.Find(ghostBaseType);
        }

        public event Action<string, string> ErrorMethodEvent;
    }
}
