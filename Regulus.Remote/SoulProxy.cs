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
        readonly List<NotifierUpdater> _TypeObjectNotifiables;
        readonly System.Action _Dispose;

        public delegate void OnPrpertySoul(long soul_id, int property_id, TypeObject type_object);
        public event OnPrpertySoul SupplySoulEvent;
        public event OnPrpertySoul UnsupplySoulEvent;
        readonly System.Collections.Concurrent.ConcurrentQueue<IPropertyIdValue> _PropertyChangeds;

        public readonly int InterfaceId;

        public SoulProxy(long id, int interface_id, Type object_type, object object_instance, System.Collections.Generic.IReadOnlyDictionary<PropertyInfo, int> property_ids)
        {
            MethodInfos = object_type.GetMethods();
            ObjectInstance = object_instance;
            ObjectType = object_type;
            InterfaceId = interface_id;
            Id = id;
            
            _PropertyChangeds = new System.Collections.Concurrent.ConcurrentQueue<IPropertyIdValue>();
            
            _EventHandlers = new List<SoulProxyEventHandler>();            
            _TypeObjectNotifiables = new List<NotifierUpdater>(_BuildNotifier(object_instance, object_type, property_ids));
            _PropertyUpdaters = new List<PropertyUpdater>(_BuildProperty(object_instance ,object_type , property_ids));


            _Regist(_PropertyUpdaters);
            _Regist(_TypeObjectNotifiables);



            _Dispose = () => {

                _Unregist(_PropertyUpdaters);
                _Unregist(_TypeObjectNotifiables);

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

                foreach (System.IDisposable item in _TypeObjectNotifiables)
                {
                    item.Dispose();
                }
                _TypeObjectNotifiables.Clear();
            };
        }

        

        private IEnumerable<Tuple<int, PropertyInfo>> _GetPropertys(Type soul_type , System.Collections.Generic.IReadOnlyDictionary<PropertyInfo, int> property_ids)
        {
            return from p in soul_type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                    select new Tuple<int, PropertyInfo>(property_ids[p] , p);            
        }

        private IEnumerable<NotifierUpdater> _BuildNotifier(object soul, Type soul_type, System.Collections.Generic.IReadOnlyDictionary<PropertyInfo, int> propertyIds)
        {
            return from p in _GetPropertys(soul_type, propertyIds)
                   let property = p.Item2
                   let id = p.Item1
                   where property.PropertyType.GetInterfaces().Any(t => t == typeof(ITypeObjectNotifiable))       
                   let propertyValue = property.GetValue(soul)
                   select new NotifierUpdater(id, propertyValue as ITypeObjectNotifiable);
        }
        private IEnumerable<PropertyUpdater> _BuildProperty(object soul, Type soul_type, System.Collections.Generic.IReadOnlyDictionary<PropertyInfo, int> propertyIds)
        {            
            foreach (var item in _GetPropertys(soul_type,propertyIds))
            {
                PropertyInfo property = item.Item2;
                var id = item.Item1;
                if (property.PropertyType.GetInterfaces().Any(t => t == typeof(IDirtyable)))
                {
                    object propertyValue = property.GetValue(soul);
                    IDirtyable dirtyable = propertyValue as IDirtyable;
                    yield return new PropertyUpdater(dirtyable, id);
                }
            }
            
        }
        private void _Regist(List<NotifierUpdater> updaters)
        {
            foreach (var updater  in updaters)
            {
                updater.SupplyEvent += _SupplySoul;
                updater.UnsupplyEvent += _UnsupplySoul;
            }
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

            _Dispose();
        }

        private void _Unregist(List<PropertyUpdater> property_updaters)
        {
            foreach (var updater in property_updaters)
            {
                updater.ChnageEvent += _UpdatePropertyChange(updater);
            }
        }

        private void _Unregist(List<NotifierUpdater> updaters)
        {
            foreach (var updater in updaters)
            {
                updater.SupplyEvent -= _SupplySoul;
                updater.UnsupplyEvent -= _UnsupplySoul;
            }
        }

        private void _UnsupplySoul(int property_id, TypeObject type_object)
        {
            UnsupplySoulEvent(Id, property_id, type_object);
        }

        private void _SupplySoul(int property_id, TypeObject type_object)
        {
            SupplySoulEvent(Id , property_id, type_object);
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
        
    }
}

