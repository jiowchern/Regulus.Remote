using System;
using System.Collections.Generic;
using System.Reflection;

namespace Regulus.Remote
{
    public partial class SoulProvider
    {
        public partial class Soul
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


					object val;
					if (handler.TryUpdate(ObjectInstance,out val))
					{
						update_property(ID, handler.Id, val);
					}
				}
			}
		}
	}
}
