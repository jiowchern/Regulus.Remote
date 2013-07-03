using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Soul
{
	class GenericEventClosure
	{
		Guid EntityId;
		string EventName;
		Action<Guid, string, object[]> InvokeEvent;
		public static Type GetDelegateType()
		{
			return typeof(Action);
		}
		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			EntityId = entity_id;
			EventName = event_name;
			InvokeEvent = invokeevent;
		}
		public void Run()
		{
			InvokeEvent(EntityId, EventName, new object[] { });
		}
	}

	class GenericEventClosure<T1>
	{
		Guid EntityId;
		string EventName;
		Action<Guid, string, object[]> InvokeEvent;
		public static Type GetDelegateType()
		{
			return typeof(Action<T1>);
		}
		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			EntityId = entity_id;
			EventName = event_name;
			InvokeEvent = invokeevent;
		}
		public void Run(T1 arg1)
		{
			InvokeEvent(EntityId, EventName, new object[] { arg1 });
		}
	}

	class GenericEventClosure<T1, T2>
	{
		Guid EntityId;
		string EventName;
		Action<Guid, string, object[]> InvokeEvent;
		public static Type GetDelegateType()
		{
			return typeof(Action<T1, T2>);
		}
		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			EntityId = entity_id;
			EventName = event_name;
			InvokeEvent = invokeevent;
		}
		public void Run(T1 arg1, T2 arg2)
		{
			InvokeEvent(EntityId, EventName, new object[] { arg1, arg2 });
		}
	}

	class GenericEventClosure<T1, T2 , T3>
	{
		Guid EntityId;
		string EventName;
		Action<Guid, string, object[]> InvokeEvent;
		public static Type GetDelegateType()
		{
			return typeof(Action<T1, T2, T3>);
		}
		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			EntityId = entity_id;
			EventName = event_name;
			InvokeEvent = invokeevent;
		}
		public void Run(T1 arg1, T2 arg2 , T3 arg3 )
		{
			InvokeEvent(EntityId, EventName, new object[] { arg1, arg2  ,arg3});
		}
	}

	class GenericEventClosure<T1, T2, T3 , T4>
	{
		Guid EntityId;
		string EventName;
		Action<Guid, string, object[]> InvokeEvent;
		public static Type GetDelegateType()
		{
			return typeof(Action<T1, T2, T3, T4>);
		}
		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			EntityId = entity_id;
			EventName = event_name;
			InvokeEvent = invokeevent;
		}
		public void Run(T1 arg1, T2 arg2, T3 arg3 , T4 arg4)
		{
			InvokeEvent(EntityId, EventName, new object[] { arg1, arg2, arg3 , arg4});
		}
	}

	class GenericEventClosure<T1, T2, T3, T4 , T5>
	{
		Guid EntityId;
		string EventName;
		Action<Guid, string, object[]> InvokeEvent;
		delegate void Action5(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
		public static Type GetDelegateType()
		{
			return typeof(Action5);
		}
		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			EntityId = entity_id;
			EventName = event_name;
			InvokeEvent = invokeevent;
		}
		public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4 ,T5 arg5)
		{
			InvokeEvent(EntityId, EventName, new object[] { arg1, arg2, arg3, arg4, arg5 });
		}
	}
}
