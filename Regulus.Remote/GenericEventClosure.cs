using System;

namespace Regulus.Remote
{
    public delegate void InvokeEventCallabck(long entity_id , int event_id ,long handler_id, object[] args);
    public class GenericEventClosure
	{
		private readonly long _EntityId;

		private readonly int _EventId;
		private readonly long _HandlerId;

		private readonly InvokeEventCallabck InvokeEvent;

		public GenericEventClosure(long entity_id, int event_id,long handler_id, InvokeEventCallabck invokeevent)
		{
			_EntityId = entity_id;
			_EventId = event_id;
			_HandlerId = handler_id;
			InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof(Action);
		}

		public void Run()
		{
			InvokeEvent(
				_EntityId, 
				_EventId,
				_HandlerId,
				new object[]
				{
				});
		}
	}

    public class GenericEventClosure<T1>
	{
		private readonly long _EntityId;

		private readonly int _EventId;
		private readonly long _HandlerId;

		private readonly InvokeEventCallabck InvokeEvent;

		public GenericEventClosure(long entity_id, int event_id, long handler_id, InvokeEventCallabck invokeevent)
		{
			_EntityId = entity_id;
			_EventId = event_id;
			_HandlerId = handler_id;
			InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof(Action<T1>);
		}

		public void Run(T1 arg1)
		{
			InvokeEvent(
				_EntityId,
				_EventId,
				_HandlerId,
				new object[]
				{
					arg1
				});
		}
	}

    public class GenericEventClosure<T1, T2>
	{
		private readonly long _EntityId;

		private readonly int _EventId;
		private readonly long _HandlerId;

		private readonly InvokeEventCallabck InvokeEvent;

		public GenericEventClosure(long entity_id, int event_id, long handler_id, InvokeEventCallabck invokeevent)
		{
			_EntityId = entity_id;
			_EventId = event_id;
			_HandlerId = handler_id;
			InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof(Action<T1, T2>);
		}

		public void Run(T1 arg1, T2 arg2)
		{
			InvokeEvent(
				_EntityId,
				_EventId,
				_HandlerId,
				new object[]
				{
					arg1, 
					arg2
				});
		}
	}

    public class GenericEventClosure<T1, T2, T3>
	{
		private readonly long _EntityId;

		private readonly int _EventId;
		private readonly long _HandlerId;

		private readonly InvokeEventCallabck InvokeEvent;

		public GenericEventClosure(long entity_id, int event_id, long handler_id, InvokeEventCallabck invokeevent)
		{
			_EntityId = entity_id;
			_EventId = event_id;
			_HandlerId = handler_id;
			InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof(Action<T1, T2, T3>);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3)
		{
			InvokeEvent(
				_EntityId,
				_EventId,
				_HandlerId,
				new object[]
				{
					arg1, 
					arg2, 
					arg3
				});
		}
	}

    public class GenericEventClosure<T1, T2, T3, T4>
	{
		private readonly long _EntityId;

		private readonly int _EventId;
		private readonly long _HandlerId;

		private readonly InvokeEventCallabck InvokeEvent;

		public GenericEventClosure(long entity_id, int event_id, long handler_id, InvokeEventCallabck invokeevent)
		{
			_EntityId = entity_id;
			_EventId = event_id;
			_HandlerId = handler_id;
			InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof(Action<T1, T2, T3, T4>);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			InvokeEvent(
				_EntityId,
				_EventId,
				_HandlerId,
				new object[]
				{
					arg1, 
					arg2, 
					arg3, 
					arg4
				});
		}
	}

    public class GenericEventClosure<T1, T2, T3, T4, T5>
	{
		private readonly long _EntityId;

		private readonly int _EventId;
		private readonly long _HandlerId;

		private readonly InvokeEventCallabck InvokeEvent;

		public GenericEventClosure(long entity_id, int event_id, long handler_id, InvokeEventCallabck invokeevent)
		{
			_EntityId = entity_id;
			_EventId = event_id;
			_HandlerId = handler_id;
			InvokeEvent = invokeevent;
		}

		public static Type GetDelegateType()
		{
			return typeof(Action);
		}

		public void Run(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			InvokeEvent(
				_EntityId,
				_EventId,
				_HandlerId,
				new object[]
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
