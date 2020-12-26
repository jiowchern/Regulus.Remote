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
        readonly System.Collections.Concurrent.ConcurrentQueue<int> _PropertyResets;

        readonly System.Collections.Concurrent.ConcurrentQueue<IPropertyIdValue> _PropertyChangeds;

        public readonly int InterfaceId;

        volatile bool _UpdateTaskEnable;
        readonly System.Threading.Tasks.Task _UpdateTask;



        public SoulProxy(long id, int interface_id, Type object_type, object object_instance,IEnumerable<PropertyUpdater> property_updaters)
        {
            MethodInfos = object_type.GetMethods();
            ObjectInstance = object_instance;
            ObjectType = object_type;
            InterfaceId = interface_id;
            Id = id;
            _PropertyUpdaters = new List<PropertyUpdater>(property_updaters);
            _PropertyResets = new System.Collections.Concurrent.ConcurrentQueue<int>();
            _PropertyChangeds = new System.Collections.Concurrent.ConcurrentQueue<IPropertyIdValue>();
            _EventHandlers = new List<SoulProxyEventHandler>();
            _UnsupplyBinder = new Dictionary<long, NotifierEventBinder>();
            _SupplyBinder = new Dictionary<long, NotifierEventBinder>();
            _UpdateTaskEnable = true;
            _UpdateTask = System.Threading.Tasks.Task.Factory.StartNew(_PropertyUpdate , System.Threading.Tasks.TaskCreationOptions.LongRunning);
        }


        internal void Release()
        {
            _UpdateTaskEnable = false;
            _UpdateTask.Wait();
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

        internal bool TryGetPropertyChange(out IPropertyIdValue property_id_value)
        {
            return _PropertyChangeds.TryDequeue(out property_id_value);
        }
        private void _PropertyUpdate()
        {


            var apr = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
            while(_UpdateTaskEnable)
            {
                int resetId;
                while (_PropertyResets.TryDequeue(out resetId))
                {
                    PropertyUpdater propertyUpdater = _PropertyUpdaters.FirstOrDefault(pu => pu.PropertyId == resetId);
                    if (propertyUpdater != null)
                        propertyUpdater.Reset();
                }



                foreach (PropertyUpdater pu in _PropertyUpdaters)
                {
                    if (pu.Update())
                    {
                        _PropertyChangeds.Enqueue(pu);
                    }

                }

                apr.Operate();
            }
            

        }

        internal void PropertyUpdateReset(int property)
        {
            _PropertyResets.Enqueue(property);
            
            

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

