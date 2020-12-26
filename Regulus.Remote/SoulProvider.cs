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
        private readonly Queue<byte[]> _EventFilter = new Queue<byte[]>();

        private readonly IRequestQueue _Peer;

        private readonly IResponseQueue _Queue;

        private readonly IProtocol _Protocol;

        private readonly EventProvider _EventProvider;

        private readonly Poller<SoulProxy> _Souls = new Poller<SoulProxy>();

        private readonly Dictionary<long, IValue> _WaitValues = new Dictionary<long, IValue>();

        private readonly ISerializer _Serializer;

        public SoulProvider(IRequestQueue peer, IResponseQueue queue, IProtocol protocol)
        {
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

        void IBinder.Return<TSoul>(TSoul soul)
        {
            if (soul == null)
            {
                throw new ArgumentNullException("soul");
            }

            _Bind(soul, true, 0);
        }

        void IBinder.Bind<TSoul>(TSoul soul)
        {
            if (soul == null)
            {
                throw new ArgumentNullException("soul");
            }

            _Bind(soul, false, 0);
        }

        void IBinder.Unbind<TSoul>(TSoul soul)
        {
            if (soul == null)
            {
                throw new ArgumentNullException("soul");
            }

            _Unbind(soul, typeof(TSoul), 0);
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
            object soul = returnValue.GetObject();
            Type type = returnValue.GetObjectType();
            SoulProxy prevSoul = (from soulInfo in _Souls.UpdateSet()
                             where object.ReferenceEquals(soulInfo.ObjectInstance, soul) && soulInfo.ObjectType == type
                             select soulInfo).SingleOrDefault();

            if (prevSoul == null)
            {
                SoulProxy new_soul = _NewSoul(soul, type);

                _LoadSoul(new_soul.InterfaceId, new_soul.Id, true);

                _LoadProperty(new_soul);

                _LoadSoulCompile(new_soul.InterfaceId, new_soul.Id, return_id, 0);
            }
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



        private void _LoadSoulCompile(int type_id, long id, long return_id, long notifier_id)
        {
            PackageLoadSoulCompile package = new PackageLoadSoulCompile();
            package.EntityId = id;
            package.ReturnId = return_id;
            package.TypeId = type_id;
            package.PassageId = notifier_id;

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

        private void _UnloadSoul(int type_id, long id, long passage)
        {

            PackageUnloadSoul package = new PackageUnloadSoul();
            package.TypeId = type_id;
            package.EntityId = id;
            package.PassageId = passage;
            _Queue.Push(ServerToClientOpCode.UnloadSoul, package.ToBuffer(_Serializer));
        }

        public void SetPropertyDone(long entityId, int property)
        {
            IEnumerable<SoulProxy> souls = from soul in _Souls.UpdateSet() where soul.Id == entityId select soul;
            SoulProxy s = souls.FirstOrDefault();
            if (s != null)
            {
                s.PropertyUpdateReset(property);
            }

        }

        private void _InvokeMethod(long entity_id, int method_id, long returnId, byte[][] args)
        {
            var soulInfo = (from soul in _Souls.UpdateSet()
                            where soul.Id == entity_id
                            select new
                            {
                                soul.MethodInfos,
                                soul.ObjectInstance
                            }).FirstOrDefault();
            if (soulInfo != null)
            {
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
        }
        public void RemoveEvent(long entity_id, int event_id, long handler_id)
        {
            SoulProxy soul = (from s in _Souls.UpdateSet() where s.Id == entity_id select s).FirstOrDefault();
            if (soul == null)
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
            SoulProxy soul = (from s in _Souls.UpdateSet() where s.Id == entity_id select s).FirstOrDefault();
            if (soul == null)
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
        public void RemoveNotifierUnsupply(long entity_id, int property, long notifier_id)
        {
            SoulProxy soul = (from s in _Souls.UpdateSet() where s.Id == entity_id select s).FirstOrDefault();
            if (soul == null)
                return;
            soul.DetachUnsupply(notifier_id);
        }
        public void RemoveNotifierSupply(long entity_id, int property, long notifier_id)
        {
            SoulProxy soul = (from s in _Souls.UpdateSet() where s.Id == entity_id select s).FirstOrDefault();
            if (soul == null)
                return;
            soul.DetachSupply(notifier_id);
        }
        public void AddNotifierSupply(long entity_id, int property_id, long notifier_id)
        {
            SoulProxy soul = (from s in _Souls.UpdateSet() where s.Id == entity_id select s).FirstOrDefault();
            if (soul == null)
                return;

            PropertyInfo propertyInfo = _Protocol.GetMemberMap().GetProperty(property_id);
            if (propertyInfo == null)
                return;

            Type gpiType = propertyInfo.PropertyType.GetGenericArguments().Single();
            NotifierEventBinder binder = NotifierEventBinder.Create(soul.ObjectInstance, propertyInfo, nameof(INotifier<object>.Supply), (gpi) => _BindSupply(gpi, gpiType, notifier_id));
            if (binder == null)
                return;
            soul.AttachSupply(notifier_id, binder);

        }

        public void AddNotifierUnsupply(long entity_id, int property_id, long notifier_id)
        {
            SoulProxy soul = (from s in _Souls.UpdateSet() where s.Id == entity_id select s).FirstOrDefault();
            if (soul == null)
                return;

            PropertyInfo propertyInfo = _Protocol.GetMemberMap().GetProperty(property_id);
            if (propertyInfo == null)
                return;
            Type gpiType = propertyInfo.PropertyType.GetGenericArguments().Single();
            NotifierEventBinder binder = NotifierEventBinder.Create(soul.ObjectInstance, propertyInfo, nameof(INotifier<object>.Unsupply), (gpi) => _UnbindSupply(gpi, gpiType, notifier_id));
            if (binder == null)
                return;

            soul.AttachUnsupply(notifier_id, binder);
        }

        private void _UnbindSupply(object gpi, Type gpiType, long notifier_id)
        {
            _Unbind(gpi, gpiType, notifier_id);
        }

        private void _BindSupply(object gpi, Type gpiType, long notifier_id)
        {
            _Bind(gpi, gpiType, false, 0, notifier_id);
        }



        private void _ErrorDeserialize(string method_name, long return_id, string message)
        {
            PackageErrorMethod package = new PackageErrorMethod();
            package.Message = message;
            package.Method = method_name;
            package.ReturnTarget = return_id;
            _Queue.Push(ServerToClientOpCode.ErrorMethod, package.ToBuffer(_Serializer));
        }

        private void _Bind<TSoul>(TSoul soul, bool return_type, long return_id)
        {
            _Bind(soul, typeof(TSoul), return_type, return_id, 0);
        }

        private void _Bind(object soul, Type soul_type, bool return_type, long return_id, long notifier_id)
        {
            SoulProxy prevSoul = (from soulInfo in _Souls.UpdateSet()
                             where object.ReferenceEquals(soulInfo.ObjectInstance, soul) && soulInfo.ObjectType == soul_type
                             select soulInfo).SingleOrDefault();

            if (prevSoul == null)
            {
                SoulProxy newSoul = _NewSoul(soul, soul_type);

                _LoadSoul(newSoul.InterfaceId, newSoul.Id, return_type);
                _LoadProperty(newSoul);
                _LoadSoulCompile(newSoul.InterfaceId, newSoul.Id, return_id, notifier_id);
            }
        }

        private SoulProxy _NewSoul(object soul, Type soul_type)
        {

            MemberMap map = _Protocol.GetMemberMap();
            int interfaceId = map.GetInterface(soul_type);
            SoulProxy newSoul = new SoulProxy(_IdLandlord.Rent(), interfaceId, soul_type, soul, _BuildProperty(soul, soul_type, map ));
            

            _Souls.Add(newSoul);

            return newSoul;
        }

        private static IEnumerable<PropertyUpdater> _BuildProperty(object soul, Type soul_type, MemberMap map)
        {
            // property 
            PropertyInfo[] propertys = soul_type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < propertys.Length; ++i)
            {
                PropertyInfo property = propertys[i];
                int id = map.GetProperty(property);

                if (property.PropertyType.GetInterfaces().Any(t => t == typeof(IDirtyable)))
                {
                    object propertyValue = property.GetValue(soul);
                    IDirtyable dirtyable = propertyValue as IDirtyable;

                    yield return new PropertyUpdater(dirtyable, id);
                    

                }
            }
        }

        private void _Unbind(object soul, Type type, long passage)
        {
            SoulProxy soulInfo = (from soul_info in _Souls.UpdateSet()
                             where object.ReferenceEquals(soul_info.ObjectInstance, soul) && soul_info.ObjectType == type
                             select soul_info).SingleOrDefault();

            if (soulInfo != null)
            {
                soulInfo.Release();

                _UnloadSoul(soulInfo.InterfaceId, soulInfo.Id, passage);
                _Souls.Remove(s => { return s == soulInfo; });
                _IdLandlord.Return(soulInfo.Id);
            }
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
            SoulProxy[] souls = _Souls.UpdateSet();


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
            SoulProxy soul = (from s in _Souls.UpdateSet() where s.Id == entityId select s).FirstOrDefault();
            if (soul != null)
            {
                _Unbind(soul.ObjectInstance, soul.ObjectType, 0);
            }
        }
    }
}
