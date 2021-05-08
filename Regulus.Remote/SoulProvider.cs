using Regulus.Serialization;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote
{
    
    
    public class SoulProvider : IDisposable, IBinder
    {
        private readonly IdLandlord _IdLandlord;
        private readonly Queue<byte[]> _EventFilter ;

        private readonly IRequestQueue _Peer;

        private readonly IResponseQueue _Queue;

        private readonly IProtocol _Protocol;

        private readonly EventProvider _EventProvider;

        private readonly System.Collections.Concurrent.ConcurrentDictionary<long,SoulProxy> _Souls ;

        private readonly Dictionary<long, IValue> _WaitValues ;

        private readonly ISerializer _Serializer;
        
        public SoulProvider(IRequestQueue peer, IResponseQueue queue, IProtocol protocol)
        {
            
            _WaitValues = new Dictionary<long, IValue>();
            _Souls = new System.Collections.Concurrent.ConcurrentDictionary<long, SoulProxy>();
            _EventFilter = new Queue<byte[]>();
            _IdLandlord = new IdLandlord();
            _Queue = queue;
            _Protocol = protocol;

            _EventProvider = protocol.GetEventProvider();

            _Serializer = protocol.GetSerialize();
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
            if (soul == null)
            {
                throw new ArgumentNullException("soul");
            }

            return _Bind(soul, false, 0);
        }

        void IBinder.Unbind(ISoul soul)
        {
            if (soul == null)
            {
                throw new ArgumentNullException("soul");
            }

            _Unbind(soul.Id);
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



            PackageInvokeEvent package = new PackageInvokeEvent();
            package.EntityId = entity_id;
            package.Event = event_id;
            package.HandlerId = handler_id;
            package.EventParams = (from a in args select _Serializer.Serialize(a)).ToArray();
            _InvokeEvent(package.ToBuffer(_Serializer));
        }

        private void _InvokeEvent(byte[] argmants)
        {
            lock (_EventFilter)
            {
                _EventFilter.Enqueue(argmants);
            }
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

            SoulProxy new_soul = _NewSoul(soul, type);

            _LoadSoul(new_soul.InterfaceId, new_soul.Id, true);

            _LoadProperty(new_soul);

            _LoadSoulCompile(new_soul.InterfaceId, new_soul.Id, return_id);
        }

        private void _LoadProperty(SoulProxy new_soul)
        {

            PropertyInfo[] propertys = new_soul.ObjectType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            MemberMap map = _Protocol.GetMemberMap();
            for (int i = 0; i < propertys.Length; ++i)
            {
                PropertyInfo property = propertys[i];
                int id = map.GetProperty(property);

                if (property.PropertyType.GetInterfaces().Any(t => t == typeof(IDirtyable)))
                {
                    object propertyValue = property.GetValue(new_soul.ObjectInstance);

                    IAccessable accessable = propertyValue as IAccessable;
                    _LoadProperty(new_soul.Id, id, accessable.Get());
                }

            }
        }



        private void _ReturnDataValue(long returnId, IValue returnValue)
        {
            object value = returnValue.GetObject();
            PackageReturnValue package = new PackageReturnValue();
            package.ReturnTarget = returnId;
            package.ReturnValue = _Serializer.Serialize(value);
            _Queue.Push(ServerToClientOpCode.ReturnValue, package.ToBuffer(_Serializer));
        }



        private void _LoadSoulCompile(int type_id, long id, long return_id)
        {
            PackageLoadSoulCompile package = new PackageLoadSoulCompile();
            package.EntityId = id;
            package.ReturnId = return_id;
            package.TypeId = type_id;            
            _Queue.Push(ServerToClientOpCode.LoadSoulCompile, package.ToBuffer(_Serializer));
        }
        private void _LoadProperty(long id, int property, object val)
        {
            PackageSetProperty package = new PackageSetProperty();
            package.EntityId = id;
            package.Property = property;
            package.Value = _Serializer.Serialize(val);
            _Queue.Push(ServerToClientOpCode.SetProperty, package.ToBuffer(_Serializer));
        }
        private void _LoadSoul(int type_id, long id, bool return_type)
        {
            PackageLoadSoul package = new PackageLoadSoul();
            package.TypeId = type_id;
            package.EntityId = id;
            package.ReturnType = return_type;
            _Queue.Push(ServerToClientOpCode.LoadSoul, package.ToBuffer(_Serializer));


        }

        private void _UnloadSoul(int type_id, long id)
        {

            PackageUnloadSoul package = new PackageUnloadSoul();            
            package.EntityId = id;
            
            _Queue.Push(ServerToClientOpCode.UnloadSoul, package.ToBuffer(_Serializer));
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
            if (methodInfo != null)
            {

                try
                {

                    IEnumerable<object> argObjects = args.Select(arg => _Serializer.Deserialize(arg));

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
        }
        public void RemoveEvent(long entity_id, int event_id, long handler_id)
        {



            SoulProxy soul;
            
            if (!_Souls.TryGetValue(entity_id, out soul))
                return;



            EventInfo eventInfo = _Protocol.GetMemberMap().GetEvent(event_id);
            if (eventInfo == null)
                return;

            if (eventInfo.DeclaringType != soul.ObjectType)
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
            if (eventInfo.DeclaringType != soul.ObjectType)
                return;

            Delegate del = _BuildDelegate(eventInfo, soul.Id, handler_id, _InvokeEvent);

            var handler = new SoulProxyEventHandler(soul.ObjectInstance, del, eventInfo, handler_id);
            soul.AddEvent(handler);

        }

        private void _ErrorDeserialize(string method_name, long return_id, string message)
        {
            PackageErrorMethod package = new PackageErrorMethod();
            package.Message = message;
            package.Method = method_name;
            package.ReturnTarget = return_id;
            _Queue.Push(ServerToClientOpCode.ErrorMethod, package.ToBuffer(_Serializer));
        }

        private ISoul _Bind<TSoul>(TSoul soul, bool return_type, long return_id)
        {
            return _Bind(soul, typeof(TSoul), return_type, return_id);
        }
        private SoulProxy _NewSoul(object soul, Type soul_type)
        {

            MemberMap map = _Protocol.GetMemberMap();
            int interfaceId = map.GetInterface(soul_type);
            SoulProxy newSoul = new SoulProxy(_IdLandlord.Rent(), interfaceId, soul_type, soul, map.Propertys.Item1s);

            _Souls.TryAdd(newSoul.Id, newSoul);

            return newSoul;
        }

        private SoulProxy _Bind(object soul, Type soul_type, bool return_type, long return_id)
        {
            SoulProxy newSoul = _NewSoul(soul, soul_type);
            newSoul.SupplySoulEvent += _PropertyBind;
            newSoul.UnsupplySoulEvent += _PropertyUnbind;            
            _LoadSoul(newSoul.InterfaceId, newSoul.Id, return_type);
            _LoadProperty(newSoul);
            _LoadSoulCompile(newSoul.InterfaceId, newSoul.Id, return_id);

            return newSoul;
        }

        private void _Unbind(long id)
        {
            /*SoulProxy soulInfo = (from soul_info in _Souls.GetConsumingEnumerable()
                                  where object.ReferenceEquals(soul_info.ObjectInstance, soul) && soul_info.ObjectType == type
                             select soul_info).SingleOrDefault();*/

            SoulProxy soulInfo;
            if (!_Souls.TryRemove(id, out soulInfo))
                throw new Exception($"can't find the soul {id} to delete.");
            
            soulInfo.SupplySoulEvent -= _PropertyBind;
            soulInfo.UnsupplySoulEvent -= _PropertyUnbind;

            soulInfo.Release();
            _UnloadSoul(soulInfo.InterfaceId, soulInfo.Id);            
            _IdLandlord.Return(soulInfo.Id);

            
        }

        private void _PropertyUnbind(long soul_id, int property_id, long property_soul_id)
        {
            
            PackagePropertySoul package = new PackagePropertySoul();
            package.OwnerId = soul_id;
            package.PropertyId = property_id;
            package.EntiryId = property_soul_id;
            _Queue.Push(ServerToClientOpCode.RemovePropertySoul, package.ToBuffer(_Serializer));

            _Unbind(property_soul_id);
            
        }

        private ISoul _PropertyBind(long soul_id , int property_id, TypeObject type_object)
        {            
            var soul = _Bind(type_object.Instance, type_object.Type, false, 0);

            PackagePropertySoul package = new PackagePropertySoul();
            package.OwnerId = soul_id;
            package.PropertyId = property_id;
            package.EntiryId = soul.Id;
            _Queue.Push(ServerToClientOpCode.AddPropertySoul, package.ToBuffer(_Serializer));
            return soul;
        }

        private Delegate _BuildDelegate(EventInfo info, long entity_id, long handler_id, InvokeEventCallabck invoke_Event)
        {

            IEventProxyCreator eventCreator = _EventProvider.Find(info);
            MemberMap map = _Protocol.GetMemberMap();
            int id = map.GetEvent(info);
            return eventCreator.Create(entity_id, id, handler_id, invoke_Event);



        }

        public void Update()
        {
            SoulProxy[] souls = _Souls.ToArray().Select(kv=>kv.Value).ToArray();


            foreach (SoulProxy soul in souls)
            {
                IPropertyIdValue change;
                while (soul.TryGetPropertyChange(out change))
                {
                    _LoadProperty(soul.Id, change.Id, change.Instance);
                }
            }

            lock (_EventFilter)
            {
                foreach (byte[] filter in _EventFilter)
                {
                    _Queue.Push(ServerToClientOpCode.InvokeEvent, filter);
                }

                _EventFilter.Clear();
            }
        }

        public void Unbind(long entityId)
        {
            _Unbind(entityId);
            
        }
    }
}
