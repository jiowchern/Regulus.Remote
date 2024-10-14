using System;
using System.Linq;
using System.Reflection;
using Regulus.Remote.Packages;
using System.Collections.Concurrent;

namespace Regulus.Remote
{

    public class SoulEventHandler
    {
        private readonly IResponseQueue _Queue;
        private readonly IProtocol _Protocol;
        private readonly ConcurrentDictionary<long, SoulProxy> _Souls;
        private readonly EventProvider _EventProvider;
        private readonly ISerializable _Serializer;
        private readonly IInternalSerializable _InternalSerializable;

        public SoulEventHandler(
            IResponseQueue queue,
            IProtocol protocol,
            ConcurrentDictionary<long, SoulProxy> souls,
            EventProvider eventProvider,
            ISerializable serializer,
            IInternalSerializable internalSerializable)
        {
            _Queue = queue;
            _Protocol = protocol;
            _Souls = souls;
            _EventProvider = eventProvider;
            _Serializer = serializer;
            _InternalSerializable = internalSerializable;
        }

        public void AddEvent(long entityId, int eventId, long handlerId)
        {
            if (!_Souls.TryGetValue(entityId, out var soul))
                return;

            var eventInfo = _Protocol.GetMemberMap().GetEvent(eventId);
            if (eventInfo == null || !soul.Is(eventInfo.DeclaringType))
                return;

            var del = _BuildDelegate(eventInfo, soul.Id, handlerId, _InvokeEvent);
            var handler = new SoulProxyEventHandler(soul.ObjectInstance, del, eventInfo, handlerId);
            soul.AddEvent(handler);
        }

        public void RemoveEvent(long entityId, int eventId, long handlerId)
        {
            if (!_Souls.TryGetValue(entityId, out var soul))
                return;

            var eventInfo = _Protocol.GetMemberMap().GetEvent(eventId);
            if (eventInfo == null || !soul.Is(eventInfo.DeclaringType))
                return;

            soul.RemoveEvent(eventInfo, handlerId);
        }

        private void _InvokeEvent(long entityId, int eventId, long handlerId, object[] args)
        {
            var info = _Protocol.GetMemberMap().GetEvent(eventId);
            var package = new PackageInvokeEvent
            {
                EntityId = entityId,
                Event = eventId,
                HandlerId = handlerId,
                EventParams = args.Zip(info.EventHandlerType.GetGenericArguments(), (arg, par) => _Serializer.Serialize(par, arg).ToArray()).ToArray()
            };
            _Queue.Push(ServerToClientOpCode.InvokeEvent, _InternalSerializable.Serialize(package));
        }

        private Delegate _BuildDelegate(EventInfo info, long entityId, long handlerId, InvokeEventCallabck invokeEvent)
        {
            var eventCreator = _EventProvider.Find(info);
            var map = _Protocol.GetMemberMap();
            int id = map.GetEvent(info);
            return eventCreator.Create(entityId, id, handlerId, invokeEvent);
        }
    }

}

