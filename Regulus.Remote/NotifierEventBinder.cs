using System;
using System.Reflection;

namespace Regulus.Remote
{
    class NotifierEventBinder
    {
		public System.Action<object> InvokeEvent;
		
        
        readonly Delegate _SupplyHandler;
        
        readonly System.Action _Dispose;
        
        public static NotifierEventBinder Create(object instance, PropertyInfo property,string event_name)
        {
            var notifierType = property.PropertyType;

            if (!notifierType.IsInterface)
            {
                return null;
            }
            if (notifierType.GetGenericTypeDefinition() != typeof(INotifier<>))
                return null;

            var notifier = property.GetValue(instance);
            return new NotifierEventBinder(notifier , notifierType.GetEvent(event_name));
        }
        public NotifierEventBinder(object notifier, EventInfo info)
        {

            var supplyEventInfo = info;            
            _SupplyHandler = new System.Action<object>(_Supply);            
            supplyEventInfo.AddEventHandler(notifier, _SupplyHandler);

            

            _Dispose = () => {
                
                supplyEventInfo.RemoveEventHandler(notifier, _SupplyHandler);
            
            };
        }

        void _Supply(object gpi)
        {
            InvokeEvent(gpi);
        }

        

        internal void Dispose()
        {
            _Dispose();
        }
    }
}
