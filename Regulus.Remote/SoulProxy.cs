using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Regulus.Remote
{
  
    public class SoulProxy
    {

        public readonly long Id;

        public readonly object ObjectInstance;

        public readonly Type ObjectType;

        public readonly MethodInfo[] MethodInfos;

        readonly List<SoulProxyEventHandler> _EventHandlers;

        readonly List<PropertyUpdater> _PropertyUpdaters;


        
        public readonly int InterfaceId;

        public SoulProxy(long id, int interface_id, Type object_type, object object_instance)
        {
            MethodInfos = object_type.GetMethods();
            ObjectInstance = object_instance;
            ObjectType = object_type;
            InterfaceId = interface_id;
            Id = id;
            _PropertyUpdaters = new List<PropertyUpdater>();        
            _EventHandlers = new List<SoulProxyEventHandler>();
            _UnsupplyBinder = new Dictionary<long, NotifierEventBinder>();
            _SupplyBinder = new Dictionary<long, NotifierEventBinder>();

        }


        internal void Release()
        {
            lock(_EventHandlers)
            {
                foreach (SoulProxyEventHandler eventHandler in _EventHandlers)
                {
                    eventHandler.Release();

                }
                _EventHandlers.Clear();
            }
            

            lock(_PropertyUpdaters)
            {
                foreach (PropertyUpdater pu in _PropertyUpdaters)
                {
                    pu.Release();
                }
                _PropertyUpdaters.Clear();
            }
            
        }

        internal IEnumerable<Tuple<int, object>> PropertyUpdate()
        {
            lock(_PropertyUpdaters)
            {
                var propertys = _PropertyUpdaters.ToArray();
                foreach (PropertyUpdater pu in propertys)
                {
                    if (pu.Update())
                        yield return new Tuple<int, object>(pu.PropertyId, pu.Value);
                }
            }
            
        }

        internal void PropertyUpdateReset(int property)
        {
            lock(_PropertyUpdaters)
            {
                PropertyUpdater propertyUpdater = _PropertyUpdaters.FirstOrDefault(pu => pu.PropertyId == property);
                if (propertyUpdater != null)
                    propertyUpdater.Reset();
            }
            

        }

        internal void AddPropertyUpdater(PropertyUpdater pu)
        {
            lock(_PropertyUpdaters)
                _PropertyUpdaters.Add(pu);
        }

        internal void AddEvent(SoulProxyEventHandler handler)
        {
            lock(_EventHandlers)
                _EventHandlers.Add(handler);
            Regulus.Utility.Log.Instance.WriteDebug($"AddEvent {handler.HandlerId}");
        }

        internal void RemoveEvent(EventInfo eventInfo, long handler_id)
        {
            Regulus.Utility.Log.Instance.WriteDebug($"RemoveEvent {handler_id}");
            lock(_EventHandlers)
            {
                SoulProxyEventHandler eventHandler = _EventHandlers.FirstOrDefault(eh => eh.HandlerId == handler_id && eh.EventInfo == eventInfo);

                _EventHandlers.Remove(eventHandler);

                eventHandler.Release();
            }
            
        }

        


        readonly Dictionary<long, NotifierEventBinder> _SupplyBinder;
        readonly Dictionary<long, NotifierEventBinder> _UnsupplyBinder;
        internal void AttachSupply(long id, NotifierEventBinder binder)
        {
            _Attach(_SupplyBinder, id, binder);
        }
        internal void DetachSupply(long id)
        {
            _Detach(_SupplyBinder, id);
        }

        internal void AttachUnsupply(long id, NotifierEventBinder binder)
        {
            _Attach(_UnsupplyBinder, id, binder);
        }
        internal void DetachUnsupply(long id)
        {
            _Detach(_UnsupplyBinder, id);
        }

        private void _Attach(Dictionary<long, NotifierEventBinder> binders, long id, NotifierEventBinder binder)
        {
            binders.Add(id, binder);
            binder.Setup();
        }



        private void _Detach(Dictionary<long, NotifierEventBinder> binders, long id)
        {
            NotifierEventBinder binder;
            if (binders.TryGetValue(id, out binder))
            {
                binders.Remove(id);
                binder.Dispose();
            }
        }
    }
}

