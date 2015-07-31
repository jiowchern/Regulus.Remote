// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericEventClosure.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the GenericEventClosure type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

#endregion

namespace Regulus.Remoting
{
	internal class GenericEventClosure
	{
		private readonly Guid EntityId;

		private readonly string EventName;

		private readonly Action<Guid, string, object[]> InvokeEvent;

		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			this.EntityId = entity_id;
			this.EventName = event_name;
			this.InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof (Action);
		}

		public void Run()
		{
			this.InvokeEvent(this.EntityId, this.EventName, new object[]
			{
			});
		}
	}

	internal class GenericEventClosure<T1>
	{
		private readonly Guid EntityId;

		private readonly string EventName;

		private readonly Action<Guid, string, object[]> InvokeEvent;

		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			this.EntityId = entity_id;
			this.EventName = event_name;
			this.InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof (Action<T1>);
		}

		public void Run(T1 arg1)
		{
			this.InvokeEvent(this.EntityId, this.EventName, new object[]
			{
				arg1
			});
		}
	}

	internal class GenericEventClosure<T1, T2>
	{
		private readonly Guid EntityId;

		private readonly string EventName;

		private readonly Action<Guid, string, object[]> InvokeEvent;

		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			this.EntityId = entity_id;
			this.EventName = event_name;
			this.InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof (Action<T1, T2>);
		}

		public void Run(T1 arg1, T2 arg2)
		{
			this.InvokeEvent(this.EntityId, this.EventName, new object[]
			{
				arg1, 
				arg2
			});
		}
	}

	internal class GenericEventClosure<T1, T2, T3>
	{
		private readonly Guid EntityId;

		private readonly string EventName;

		private readonly Action<Guid, string, object[]> InvokeEvent;

		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			this.EntityId = entity_id;
			this.EventName = event_name;
			this.InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof (Action<T1, T2, T3>);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3)
		{
			this.InvokeEvent(this.EntityId, this.EventName, new object[]
			{
				arg1, 
				arg2, 
				arg3
			});
		}
	}

	internal class GenericEventClosure<T1, T2, T3, T4>
	{
		private readonly Guid EntityId;

		private readonly string EventName;

		private readonly Action<Guid, string, object[]> InvokeEvent;

		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			this.EntityId = entity_id;
			this.EventName = event_name;
			this.InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof (Action<T1, T2, T3, T4>);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			this.InvokeEvent(this.EntityId, this.EventName, new object[]
			{
				arg1, 
				arg2, 
				arg3, 
				arg4
			});
		}
	}

	internal class GenericEventClosure<T1, T2, T3, T4, T5>
	{
		private readonly Guid EntityId;

		private readonly string EventName;

		private readonly Action<Guid, string, object[]> InvokeEvent;

		public GenericEventClosure(Guid entity_id, string event_name, Action<Guid, string, object[]> invokeevent)
		{
			this.EntityId = entity_id;
			this.EventName = event_name;
			this.InvokeEvent = invokeevent;
		}

		private delegate void Action5(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

		public static Type GetDelegateType()
		{
			return typeof (Action5);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			this.InvokeEvent(this.EntityId, this.EventName, new object[]
			{
				arg1, 
				arg2, 
				arg3, 
				arg4, 
				arg5
			});
		}
	}
}