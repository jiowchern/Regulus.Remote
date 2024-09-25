using Regulus.Memorys;
using Regulus.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote
{


    /*public class SoulProvider2 : IDisposable, IBinder
    {
        private readonly IdLandlord _IdLandlord;
        

        private readonly IRequestQueue _Peer;

        private readonly IResponseQueue _Queue;

        private readonly IProtocol _Protocol;

        private readonly EventProvider _EventProvider;

        private readonly System.Collections.Concurrent.ConcurrentDictionary<long,SoulProxy> _Souls ;

        private readonly Dictionary<long, IValue> _WaitValues ;

        private readonly ISerializable _Serializer;
        readonly IInternalSerializable _InternalSerializable;
        
        public SoulProvider2(IRequestQueue peer, IResponseQueue queue, IProtocol protocol, ISerializable serializable, IInternalSerializable internal_serializable )
        {
            _InternalSerializable = internal_serializable;
            _WaitValues = new Dictionary<long, IValue>();
            _Souls = new System.Collections.Concurrent.ConcurrentDictionary<long, SoulProxy>();
            
            _IdLandlord = new IdLandlord();
            _Queue = queue;
            _Protocol = protocol;

            _EventProvider = protocol.GetEventProvider();

            _Serializer = serializable;
            _Peer = peer;
            _Peer.InvokeMethodEvent += _InvokeMethod;
        }

        public void Dispose()
        {
            _Peer.InvokeMethodEvent -= _InvokeMethod;
        }

        ISoul IBinder.Return<TSoul>(TSoul soul)
        {
            if (soul == null)
            {
                throw new ArgumentNullException("soul");
            }

            return _Bind(soul, true, 0);
        }

        ISoul IBinder.Bind<TSoul>(TSoul soul)
        {
            

            return _Bind(soul, false, 0);
        }

        void IBinder.Unbind(ISoul soul)
        {

            
            _Unbind(soul);
        }

        event Action IBinder.BreakEvent
        {
            add
            {
                lock (_Peer)
                {
                    _Peer.BreakEvent += value;
                }
            }

            remove
            {
                lock (_Peer)
                {
                    _Peer.BreakEvent -= value;
                }
            }
        }



        private void _InvokeEvent(long entity_id, int event_id, long handler_id, object[] args)
        {
            var info = _Protocol.GetMemberMap().GetEvent(event_id);
            Regulus.Remote.Packages.PackageInvokeEvent package = new Regulus.Remote.Packages.PackageInvokeEvent();
            package.EntityId = entity_id;
            package.Event = event_id;
            package.HandlerId = handler_id;
            
            package.EventParams = args.Zip(info.EventHandlerType.GetGenericArguments(), (arg, par) => _Serializer.Serialize(par, arg).ToArray()).ToArray();
            
            _InvokeEvent(_InternalSerializable.Serialize(package).ToArray());
        }

        private void _InvokeEvent(byte[] argmants)
        {
            _Queue.Push(ServerToClientOpCode.InvokeEvent, argmants);            
        }

        private void _ReturnValue(long returnId, IValue returnValue)
        {
            IValue outValue = null;
            if (_WaitValues.TryGetValue(returnId, out outValue))
            {
                return;
            }

            _WaitValues.Add(returnId, returnValue);
            returnValue.QueryValue(
                obj =>
                {
                    if (returnValue.IsInterface() == false)
                    {
                        _ReturnDataValue(returnId, returnValue);
                    }
                    else
                    {
                        _ReturnSoulValue(returnId, returnValue);
                    }

                    _WaitValues.Remove(returnId);
                });
        }

        private void _ReturnSoulValue(long return_id, IValue returnValue)
        {            
            var soul = returnValue.GetObject() ;
            Type type = returnValue.GetObjectType();

            _Bind(soul, type, true, return_id);            
        }

        private void _LoadProperty(SoulProxy new_soul)
        {

            IEnumerable<PropertyInfo> propertys = new_soul.PropertyInfos;
            MemberMap map = _Protocol.GetMemberMap();
            foreach (PropertyInfo property in propertys)
            {
                
                int id = map.GetProperty(property);

                if (property.PropertyType.GetInterfaces().Any(t => t == typeof(IDirtyable)))
                {
                    object propertyValue = property.GetValue(new_soul.ObjectInstance);

                    IAccessable accessable = propertyValue as IAccessable;
                    _LoadProperty(new_soul.Id, id, accessable.Get());
                }

            }
        }
        private void _ReturnDataValue(long return_id, IValue return_value)
        {
            object value = return_value.GetObject();
            Regulus.Remote.Packages.PackageReturnValue package = new Regulus.Remote.Packages.PackageReturnValue();
            package.ReturnTarget = return_id;
            package.ReturnValue = _Serializer.Serialize(return_value.GetObjectType() , value).ToArray();
            _Queue.Push(ServerToClientOpCode.ReturnValue, _InternalSerializable.Serialize(package).ToArray());
        }



        private void _LoadSoulCompile(int type_id, long id, long return_id)
        {
            Regulus.Remote.Packages.PackageLoadSoulCompile package = new Regulus.Remote.Packages.PackageLoadSoulCompile();
            package.EntityId = id;
            package.ReturnId = return_id;
            package.TypeId = type_id;            
            _Queue.Push(ServerToClientOpCode.LoadSoulCompile, _InternalSerializable.Serialize(package).ToArray());
        }
        private void _LoadProperty(long id, int property , object val)
        {
            var info = _Protocol.GetMemberMap().GetProperty(property);
            Regulus.Remote.Packages.PackageSetProperty package = new Regulus.Remote.Packages.PackageSetProperty();
            package.EntityId = id;
            package.Property = property;
            package.Value = _Serializer.Serialize(info.PropertyType, val).ToArray();            
            _Queue.Push(ServerToClientOpCode.SetProperty, _InternalSerializable.Serialize(package).ToArray());
        }
        private void _LoadSoul(int type_id, long id, bool return_type)
        {
            Regulus.Remote.Packages.PackageLoadSoul package = new Regulus.Remote.Packages.PackageLoadSoul();
            package.TypeId = type_id;
            package.EntityId = id;
            package.ReturnType = return_type;
            _Queue.Push(ServerToClientOpCode.LoadSoul, _InternalSerializable.Serialize(package).ToArray());


        }

        private void _UnloadSoul(int type_id, long id)
        {

            Regulus.Remote.Packages.PackageUnloadSoul package = new Regulus.Remote.Packages.PackageUnloadSoul();            
            package.EntityId = id;
            
            _Queue.Push(ServerToClientOpCode.UnloadSoul, _InternalSerializable.Serialize(package).ToArray());
        }

        public void SetPropertyDone(long entityId, int property)
        {
            

            SoulProxy soul;
            if (!_Souls.TryGetValue(entityId, out soul))
                return;
            soul.PropertyUpdateReset(property);
        }

        private void _InvokeMethod(long entity_id, int method_id, long returnId, byte[][] args)
        {
          
            SoulProxy soul;
            if (!_Souls.TryGetValue(entity_id, out soul))
                return;

            var soulInfo = new
            {
                soul.MethodInfos,
                soul.ObjectInstance
            };

            MethodInfo info = _Protocol.GetMemberMap().GetMethod(method_id);
            MethodInfo methodInfo =
                (from m in soulInfo.MethodInfos where m == info && m.GetParameters().Count() == args.Count() select m)
                    .FirstOrDefault();
            if (methodInfo == null)
                return;
            try
            {

                IEnumerable<object> argObjects = args.Zip(methodInfo.GetParameters(), (arg, par) => _Serializer.Deserialize(par.ParameterType, arg.AsBuffer()));

                object returnValue = methodInfo.Invoke(soulInfo.ObjectInstance, argObjects.ToArray());
                if (returnValue != null)
                {
                    _ReturnValue(returnId, returnValue as IValue);
                }
            }
            catch (DeserializeException deserialize_exception)
            {
                string message = deserialize_exception.Base.ToString();
                _ErrorDeserialize(method_id.ToString(), returnId, message);
            }
            catch (Exception e)
            {
                Log.Instance.WriteDebug(e.ToString());
                _ErrorDeserialize(method_id.ToString(), returnId, e.Message);
            }
        }
        public void RemoveEvent(long entity_id, int event_id, long handler_id)
        {
            SoulProxy soul;
            
            if (!_Souls.TryGetValue(entity_id, out soul))
                return;

            EventInfo eventInfo = _Protocol.GetMemberMap().GetEvent(event_id);
            if (eventInfo == null)
                return;

            
            if (!soul.Is(eventInfo.DeclaringType))
                return;

            soul.RemoveEvent(eventInfo, handler_id);
        }

        public void AddEvent(long entity_id, int event_id, long handler_id)
        {


            SoulProxy soul;
            if (!_Souls.TryGetValue(entity_id, out soul))
                return;

            EventInfo eventInfo = _Protocol.GetMemberMap().GetEvent(event_id);
            if (eventInfo == null)
                return;
            if (!soul.Is(eventInfo.DeclaringType))
                return;

            Delegate del = _BuildDelegate(eventInfo, soul.Id, handler_id, _InvokeEvent);

            var handler = new SoulProxyEventHandler(soul.ObjectInstance, del, eventInfo, handler_id);
            soul.AddEvent(handler);

        }

        private void _ErrorDeserialize(string method_name, long return_id, string message)
        {
            Regulus.Remote.Packages.PackageErrorMethod package = new Regulus.Remote.Packages.PackageErrorMethod();
            package.Message = message;
            package.Method = method_name;
            package.ReturnTarget = return_id;
            
            _Queue.Push(ServerToClientOpCode.ErrorMethod, _InternalSerializable.Serialize(package).ToArray());
        }

        private ISoul _Bind<TSoul>(TSoul soul, bool return_type, long return_id)
        {
            return _Bind(soul, typeof(TSoul), return_type, return_id);
        }
        private SoulProxy _NewSoul(object soul, Type soul_type)
        {

            
            int interfaceId = _Protocol.GetMemberMap().GetInterface(soul_type);
            var newSoul = new SoulProxy(_IdLandlord.Rent(), interfaceId, soul_type, soul );
            newSoul.SupplySoulEvent += _PropertyBind;
            newSoul.UnsupplySoulEvent += _PropertyUnbind;
            newSoul.PropertyChangedEvent += _LoadProperty;
            Regulus.Utility.Log.Instance.WriteInfo($"soul add {newSoul.Id}:{soul_type.Name}.");
            _Souls.TryAdd(newSoul.Id, newSoul);

            return newSoul;
        }

        private void _LoadProperty(long soul, IPropertyIdValue prop)
        {
            _LoadProperty(soul, prop.Id, prop.Instance);
        }

        private SoulProxy _Bind(object soul, Type soul_type, bool return_type, long return_id)
        {
            SoulProxy newSoul = _NewSoul(soul, soul_type);
            
            
            _LoadSoul(newSoul.InterfaceId, newSoul.Id, return_type);
            _LoadProperty(newSoul);
            _LoadSoulCompile(newSoul.InterfaceId, newSoul.Id, return_id);            
            newSoul.Initial(_Protocol.GetMemberMap().Propertys.Item1s);
            //Regulus.Utility.Log.Instance.WriteInfoNoDelay($"bind i:{soul.GetHashCode()} t:{soul_type} id:{newSoul.Id}");
            return newSoul;
        }

        private void _Unbind(ISoul soul)
        {
            
            SoulProxy soulInfo;
            //Regulus.Utility.Log.Instance.WriteInfoImmediate($"unbind i:{soul.Instance.GetHashCode()} t:{soul.Instance.GetType()} id:{soul.Id}.");
            if (!_Souls.TryRemove(soul.Id, out soulInfo))
                throw new Exception($"can't find the soul {soul.Id} to delete.");
            soulInfo.Release();

            soulInfo.SupplySoulEvent -= _PropertyBind;
            soulInfo.UnsupplySoulEvent -= _PropertyUnbind;
            soulInfo.PropertyChangedEvent -= _LoadProperty;



            _UnloadSoul(soulInfo.InterfaceId, soulInfo.Id);            
            _IdLandlord.Return(soulInfo.Id);
            
        }

        private void _PropertyUnbind(long soul_id, int property_id, long property_soul_id)
        {

            Regulus.Remote.Packages.PackagePropertySoul package = new Regulus.Remote.Packages.PackagePropertySoul();
            package.OwnerId = soul_id;
            package.PropertyId = property_id;
            package.EntityId = property_soul_id;
            _Queue.Push(ServerToClientOpCode.RemovePropertySoul, _InternalSerializable.Serialize(package).ToArray());

            SoulProxy soul;
            _Souls.TryGetValue(property_soul_id , out soul);
            _Unbind(soul);
            
        }
        
        private ISoul _PropertyBind(long soul_id , int property_id, TypeObject type_object)
        {            
            var soul = _Bind(type_object.Instance, type_object.Type, false, 0);

            Regulus.Remote.Packages.PackagePropertySoul package = new Regulus.Remote.Packages.PackagePropertySoul();
            package.OwnerId = soul_id;
            package.PropertyId = property_id;
            package.EntityId = soul.Id;            
            _Queue.Push(ServerToClientOpCode.AddPropertySoul, _InternalSerializable.Serialize(package).ToArray());
        
            return soul;
        }

        private Delegate _BuildDelegate(EventInfo info, long entity_id, long handler_id, InvokeEventCallabck invoke_Event)
        {

            IEventProxyCreater eventCreater = _EventProvider.Find(info);
            MemberMap map = _Protocol.GetMemberMap();
            int id = map.GetEvent(info);
            return eventCreater.Create(entity_id, id, handler_id, invoke_Event);



        }

        

        public void Unbind(long entityId)
        {
            SoulProxy soul;
            _Souls.TryGetValue(entityId, out soul);
            _Unbind(soul);
            
        }
    }*/

    public class SoulProvider : IDisposable, IBinder
    {
        private readonly IdLandlord _IdLandlord;
        private readonly IRequestQueue _Peer;
        private readonly IResponseQueue _Queue;
        private readonly IProtocol _Protocol;
        private readonly EventProvider _EventProvider;
        private readonly ConcurrentDictionary<long, SoulProxy> _Souls;
        private readonly Dictionary<long, IValue> _WaitValues;
        private readonly ISerializable _Serializer;
        private readonly IInternalSerializable _InternalSerializable;

        private readonly SoulBindHandler _bindHandler;
        private readonly SoulMethodHandler _methodHandler;
        private readonly SoulEventHandler _eventHandler;
        private readonly SoulPropertyHandler _propertyHandler;

        public SoulProvider(
            IRequestQueue peer,
            IResponseQueue queue,
            IProtocol protocol,
            ISerializable serializer,
            IInternalSerializable internalSerializable)
        {
            _IdLandlord = new IdLandlord();
            _Peer = peer;
            _Queue = queue;
            _Protocol = protocol;
            _Serializer = serializer;
            _InternalSerializable = internalSerializable;

            _EventProvider = protocol.GetEventProvider();
            _Souls = new ConcurrentDictionary<long, SoulProxy>();
            _WaitValues = new Dictionary<long, IValue>();

            _bindHandler = new SoulBindHandler(_IdLandlord, _Queue, _Protocol, _Souls, _Serializer, _InternalSerializable, _EventProvider);
            _methodHandler = new SoulMethodHandler(_Peer, _Queue, _Protocol, _Souls, _WaitValues, _Serializer, _InternalSerializable);
            _eventHandler = new SoulEventHandler(_Queue, _Protocol, _Souls, _EventProvider, _Serializer, _InternalSerializable);
            _propertyHandler = new SoulPropertyHandler(_Queue, _Protocol, _Souls, _Serializer, _InternalSerializable);
        }

        public void Dispose()
        {
            _methodHandler.Dispose();
        }

        ISoul IBinder.Return<TSoul>(TSoul soul)
        {
            return _bindHandler.Return(soul);
        }

        ISoul IBinder.Bind<TSoul>(TSoul soul)
        {
            return _bindHandler.Bind(soul);
        }

        void IBinder.Unbind(ISoul soul)
        {
            _bindHandler.Unbind(soul);
        }

        public void AddEvent(long entity, int @event, long handler)
        {
            _eventHandler.AddEvent(entity, @event, handler);
        }

        public void RemoveEvent(long entity, int @event, long handler)
        {
            _eventHandler.RemoveEvent(entity, @event, handler);
        }

        public void SetPropertyDone(long entityId, int property)
        {
            _propertyHandler.SetPropertyDone(entityId, property);
        }

        public void Unbind(long entityId)
        {
            SoulProxy soul;
            _Souls.TryGetValue(entityId, out soul);
            _bindHandler.Unbind(soul);            

        }
        

        event Action IBinder.BreakEvent
        {
            add { _Peer.BreakEvent += value; }
            remove { _Peer.BreakEvent -= value; }
        }

        // 其他需要實現的接口方法或事件處理可以在這裡添加
    }

}
