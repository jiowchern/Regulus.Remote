using System;

namespace Regulus.Remote
{
    public delegate void InvokeEventCallabck(long entity_id, int event_id, long handler_id, object[] args);
    public class GenericEventClosure
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

        public void Run(params object[] objs)
        {
            InvokeEvent(
                _EntityId,
                _EventId,
                _HandlerId,
                objs);
        }
    }

    
}
