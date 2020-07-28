using System;
using System.Reflection;

namespace Regulus.Remote
{
    class NotifierBinder
    {
		public System.Action<object> SupplyEvent;
		public System.Action<object> UnsupplyEvent;
        
        readonly Delegate _SupplyHandler;
        readonly Delegate _UnsupplyHandler;
        readonly System.Action _Dispose;
        
        public static NotifierBinder Create(object instance, PropertyInfo property)
        {
            var notifierType = property.PropertyType;

            if (!notifierType.IsInterface)
            {
                return null;
            }
            if (notifierType.GetGenericTypeDefinition() != typeof(INotifier<>))
                return null;

            var notifier = property.GetValue(instance);
            return new NotifierBinder(notifier , notifierType);
        }
        public NotifierBinder(object notifier, Type notifier_type)
        {

            var supplyEventInfo = notifier_type.GetEvent(nameof(INotifier<object>.Supply));            
            _SupplyHandler = new System.Action<object>(_Supply);            
            supplyEventInfo.AddEventHandler(notifier, _SupplyHandler);

            var unsupplyEventInfo = notifier_type.GetEvent(nameof(INotifier<object>.Unsupply));            
            _UnsupplyHandler = new System.Action<object>(_Unsupply);            
            unsupplyEventInfo.AddEventHandler(notifier, _UnsupplyHandler);

            _Dispose = () => {
                
                supplyEventInfo.RemoveEventHandler(notifier, _SupplyHandler);
                unsupplyEventInfo.RemoveEventHandler(notifier, _UnsupplyHandler);
            };
        }

        void _Supply(object gpi)
        {
            SupplyEvent(gpi);
        }

        void _Unsupply(object gpi)
        {
            UnsupplyEvent(gpi);
        }

        internal void Dispose()
        {
            _Dispose();
        }
    }
}
