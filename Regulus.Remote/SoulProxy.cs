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
        

        readonly System.Collections.Concurrent.ConcurrentQueue<IPropertyIdValue> _PropertyChangeds;

        public readonly int InterfaceId;

        



        public SoulProxy(long id, int interface_id, Type object_type, object object_instance,IEnumerable<PropertyUpdater> property_updaters)
        {
            MethodInfos = object_type.GetMethods();
            ObjectInstance = object_instance;
            ObjectType = object_type;
            InterfaceId = interface_id;
            Id = id;
            _PropertyUpdaters = new List<PropertyUpdater>(property_updaters);
            
            _PropertyChangeds = new System.Collections.Concurrent.ConcurrentQueue<IPropertyIdValue>();
            _EventHandlers = new List<SoulProxyEventHandler>();
            _UnsupplyBinder = new Dictionary<long, NotifierEventBinder>();
            _SupplyBinder = new Dictionary<long, NotifierEventBinder>();
            
            


            _Regist(_PropertyUpdaters);
        }

        private void _Regist(List<PropertyUpdater> property_updaters)
        {
            foreach (var updater in property_updaters)
            {
                updater.ChnageEvent += _UpdatePropertyChange(updater);
            }
        }

        private Action<object> _UpdatePropertyChange(PropertyUpdater updater)
        {
            return (o) => _PropertyChangeds.Enqueue(updater);
        }

        internal void Release()
        {
            _Unregist(_PropertyUpdaters);            
            
            lock (_EventHandlers)
            {
                foreach (SoulProxyEventHandler eventHandler in _EventHandlers)
                {
                    eventHandler.Release();

                }
                _EventHandlers.Clear();
            }


            foreach (PropertyUpdater pu in _PropertyUpdaters)
            {
                pu.Release();
            }
            _PropertyUpdaters.Clear();

        }

        private void _Unregist(List<PropertyUpdater> property_updaters)
        {
            foreach (var updater in property_updaters)
            {
                updater.ChnageEvent += _UpdatePropertyChange(updater);
            }
        }

        internal bool TryGetPropertyChange(out IPropertyIdValue property_id_value)
        {
            return _PropertyChangeds.TryDequeue(out property_id_value);
        }
        

        internal void PropertyUpdateReset(int property)
        {
            

            foreach (var updater in _PropertyUpdaters)
            {
                if(updater.PropertyId == property)
                {
                    updater.Reset();
                }
            }

        }

        

        internal void AddEvent(SoulProxyEventHandler handler)
        {
            lock(_EventHandlers)
                _EventHandlers.Add(handler);
            
        }

        internal void RemoveEvent(EventInfo eventInfo, long handler_id)
        {
            
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

