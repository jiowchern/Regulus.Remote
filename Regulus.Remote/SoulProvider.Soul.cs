using System;
using System.Collections.Generic;
using System.Reflection;

namespace Regulus.Remote
{
    public partial class SoulProvider
    {
        public  class Soul
		{

			public readonly long Id;

			public readonly object ObjectInstance;

			public readonly Type ObjectType;

			public readonly MethodInfo[] MethodInfos;

			public readonly List<EventHandler> EventHandlers;

			public readonly List<PropertyUpdater> PropertyUpdaters;
			public readonly int InterfaceId;

		    public Soul(long id,int interface_id,Type object_type,object object_instance , MethodInfo[] methodInfos)
			{
				MethodInfos = methodInfos;
				ObjectInstance = object_instance;
				ObjectType = object_type;
				InterfaceId = interface_id;
				Id = id;
				PropertyUpdaters = new List<PropertyUpdater>();
				EventHandlers = new List<EventHandler>();
			}

			
			

			public class EventHandler
			{
				public Delegate DelegateObject;

				public EventInfo EventInfo;
			}
		}
	}
}
