using System;
using System.Collections.Generic;
using System.Reflection;

namespace Regulus.Remote
{
    public partial class SoulProvider
    {
        private partial class Soul
		{

			public Guid ID { get; set; }

			public object ObjectInstance { get; set; }

			public Type ObjectType { get; set; }

			public MethodInfo[] MethodInfos { get; set; }

			public List<EventHandler> EventHandlers { get; set; }

			public PropertyHandler[] PropertyHandlers { get; set; }

		    public int InterfaceId { get; set; }

		    internal void ProcessDiffentValues(Action<Guid, int, object> update_property)
			{
				foreach(var handler in PropertyHandlers)
				{
					var val = handler.PropertyInfo.GetValue(ObjectInstance, null);

					if(handler.UpdateProperty(val))
					{
						if(update_property != null)
						{
							update_property(ID, handler.Id, val);
						}
					}
				}
			}
		}
	}
}
