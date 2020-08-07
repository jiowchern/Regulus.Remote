using System;
using System.Collections.Generic;
using System.Reflection;

namespace Regulus.Remote
{
    public partial class SoulProvider
    {
        public partial class Soul
        {
            public class EventHandler
            {
                readonly Delegate _DelegateInstance;

                public readonly EventInfo EventInfo;

                readonly object _Instance;

                public readonly long HandlerId;


                public EventHandler(object instance, Delegate delegate_object, EventInfo event_info, long handler_id)
                {
                    HandlerId = handler_id;
                    _Instance = instance;
                    _DelegateInstance = delegate_object;
                    EventInfo = event_info;

                    MethodInfo addMethod = event_info.GetAddMethod();
                    addMethod.Invoke(instance, new[] { _DelegateInstance });
                }



                internal void Release()
                {
                    EventInfo.RemoveEventHandler(_Instance, _DelegateInstance);

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
}
