using System;
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
                

                public EventHandler(object instance,Delegate delegate_object , EventInfo event_info ,long handler_id)
                {
					HandlerId = handler_id;
					_Instance = instance;
					_DelegateInstance = delegate_object;
					EventInfo = event_info;

					var addMethod = event_info.GetAddMethod();
					addMethod.Invoke(instance, new[] { _DelegateInstance });
				}

				

                internal void Release()
                {
					EventInfo.RemoveEventHandler(_Instance, _DelegateInstance);
					
                }
            }
        }
	}
}
