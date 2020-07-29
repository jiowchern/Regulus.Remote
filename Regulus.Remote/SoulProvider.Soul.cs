using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote
{
    public partial class SoulProvider
    {
        public partial class Soul
		{

			public readonly long Id;

			public readonly object ObjectInstance;

			public readonly Type ObjectType;

			public readonly MethodInfo[] MethodInfos;

			readonly List<EventHandler> _EventHandlers;

			readonly List<PropertyUpdater> _PropertyUpdaters;

			
			public readonly IReadOnlyCollection<PropertyUpdater> Propertys;
			public readonly int InterfaceId;

		    public Soul(long id,int interface_id,Type object_type,object object_instance )
			{
				MethodInfos = object_type.GetMethods();
				ObjectInstance = object_instance;
				ObjectType = object_type;
				InterfaceId = interface_id;
				Id = id;
				_PropertyUpdaters = new List<PropertyUpdater>();
				Propertys = _PropertyUpdaters;
				_EventHandlers = new List<EventHandler>();
				_UnsupplyBinder = new Dictionary<long, NotifierEventBinder>();
				_SupplyBinder = new Dictionary<long, NotifierEventBinder>();
				
			}
            

            internal void Release()
			{
				foreach (var eventHandler in _EventHandlers)
				{
					eventHandler.Release();

				}
				_EventHandlers.Clear();


				foreach (var pu in _PropertyUpdaters)
				{
					pu.Release();
				}
				_PropertyUpdaters.Clear();
			}

            internal IEnumerable<Tuple<int,object>> PropertyUpdate()
            {
                foreach (var pu in _PropertyUpdaters)
                {
					if (pu.Update())
						yield return new Tuple<int,object>(pu.PropertyId , pu.Value);
                }
            }

            internal void PropertyUpdateReset(int property)
            {
				var propertyUpdater =_PropertyUpdaters.FirstOrDefault(pu => pu.PropertyId == property);
				if (propertyUpdater != null)
					propertyUpdater.Reset();

			}

            internal void AddPropertyUpdater(PropertyUpdater pu)
            {
				_PropertyUpdaters.Add(pu);
            }

            internal void AddEvent(EventHandler handler)
            {
				_EventHandlers.Add(handler);
            }

            internal void RemoveEvent(EventInfo eventInfo, long handler_id)
            {
				var eventHandler = _EventHandlers.FirstOrDefault(eh => eh.HandlerId == handler_id && eh.EventInfo == eventInfo);
				_EventHandlers.Remove(eventHandler);
				eventHandler.Release();				
			}
        }
	}
}
