using Regulus.Remote.Extensions;
using Regulus.Remote.Packages;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Regulus.Remote
{
    namespace ProviderHelper
    {
        public class GhostsHandler : ClientExchangeable
        {
            private readonly Dictionary<long, GhostResponseHandler> _GhostResponseHandlers;
            
            private readonly AutoRelease<long, IGhost> _AutoRelease;
            private readonly IProtocol _Protocol;
            private readonly ISerializable _Serializer;
            private readonly IInternalSerializable _InternalSerializer;
            
            private readonly GhostsOwner _GhostsOwner;
            private readonly GhostsReturnValueHandler _ReturnValueHandler;
            private readonly InterfaceProvider _InterfaceProvider;
            

            public GhostsHandler(
                IProtocol protocol,
                ISerializable serializer,
                IInternalSerializable internalSerializer,                
                GhostsOwner ghosts_owner,
                GhostsReturnValueHandler returnValueHandler)
            {
                _Protocol = protocol;
                _Serializer = serializer;
                _InternalSerializer = internalSerializer;                
                _GhostsOwner = ghosts_owner;
                _ReturnValueHandler = returnValueHandler;
                _InterfaceProvider = _Protocol.GetInterfaceProvider();
                _GhostResponseHandlers = new Dictionary<long, GhostResponseHandler>();
                _AutoRelease = new AutoRelease<long, IGhost>();                
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

            public void LoadSoul(int typeId, long id, bool returnType)
            {
                var map = _Protocol.GetMemberMap();
                var type = map.GetInterface(typeId);
                var ghost = BuildGhost(type, id, returnType);

                var gm = new GhostMethodHandler(ghost.GetID(), _ReturnValueHandler, _Protocol, _Serializer, _InternalSerializer);
                ghost.CallMethodEvent += gm.Run;
                ClientExchangeable gmec = gm ;
                gmec.ResponseEvent += _ResponseEvent;

                var gem = new GhostEventMoveHandler(ghost.GetID(), _Protocol, _InternalSerializer);
                ghost.AddEventEvent += gem.Add;
                ghost.RemoveEventEvent += gem.Remove;
                ClientExchangeable gemec = gem;
                gemec.ResponseEvent += _ResponseEvent;



                lock (_GhostResponseHandlers)
                    _GhostResponseHandlers.Add(ghost.GetID(), new GhostResponseHandler(new WeakReference<IGhost>(ghost), _Protocol.GetMemberMap(), _Serializer));

                if (ghost.IsReturnType())
                {
                    _AutoRelease.Push(ghost.GetID(), ghost);
                }
                else
                {
                    var provider = _GhostsOwner.QueryProvider(type);
                    provider.Add(ghost);
                }
            }

            public void UnloadSoul(long id)
            {
                _GhostsOwner.RemoveGhost(id);
                lock (_GhostResponseHandlers)
                    _GhostResponseHandlers.Remove(id);
                
            }

            public void UpdateSetProperty(long entityId, int propertyId, byte[] payload)
            {
                var ghostHandler = _FindHandler(entityId);
                if (ghostHandler == null)
                    return;
                ghostHandler.UpdateSetProperty(propertyId, payload);

                var pkg = new PackageSetPropertyDone
                {
                    EntityId = entityId,
                    Property = propertyId
                };
                _ResponseEvent(ClientToServerOpCode.UpdateProperty, _InternalSerializer.Serialize(pkg));
            }

            public void InvokeEvent(long ghostId, int eventId, long handlerId, byte[][] eventParams)
            {
                var ghostHandler = _FindHandler(ghostId);
                if (ghostHandler == null)
                    return;
                ghostHandler.InvokeEvent(eventId, handlerId, eventParams);
            }

            public void AddPropertySoul(PackagePropertySoul data)
            {
                _PropertySoulAccesser(data, a => a.Add);
            }

            public void RemovePropertySoul(PackagePropertySoul data)
            {
                _PropertySoulAccesser(data, a => a.Remove);
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

                oper.Execute().Invoke(owner_handler.GetAccesser(data.PropertyId), new object[] { entity_ghost.GetInstance() });
            }
            internal GhostResponseHandler FindHandler(long entityId)
            {
                return _FindHandler(entityId);
            }
            private GhostResponseHandler _FindHandler(long ghostId)
            {
                lock (_GhostResponseHandlers)
                {
                    if (_GhostResponseHandlers.TryGetValue(ghostId, out var handler))
                    {
                        if (handler.IsValid())
                            return handler;
                    }
                }
                return null;
            }

            private IGhost BuildGhost(Type ghostBaseType, long id, bool returnType)
            {
                Type ghostType = _InterfaceProvider.Find(ghostBaseType);
                ConstructorInfo constructor = ghostType.GetConstructor(new[] { typeof(long), typeof(bool) });
                object o = constructor.Invoke(new object[] { id, returnType });
                return (IGhost)o;
            }

            public void UpdateAutoRelease()
            {
                foreach (var id in _AutoRelease.NoExist())
                {
                    var pkg = new PackageRelease { EntityId = id };
                    _ResponseEvent(ClientToServerOpCode.Release, _InternalSerializer.Serialize(pkg));
                    UnloadSoul(id);
                }
            }

            public void ClearGhosts()
            {
                lock (_GhostResponseHandlers)
                    _GhostResponseHandlers.Clear();
            }

            void Exchangeable<ServerToClientOpCode, ClientToServerOpCode>.Request(ServerToClientOpCode code, Memorys.Buffer args)
            {
                //throw new NotImplementedException();
            }
        }

    }
}
