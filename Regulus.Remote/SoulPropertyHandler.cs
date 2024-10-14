using System.Collections.Concurrent;

namespace Regulus.Remote
{
    public class SoulPropertyHandler
    {
        private readonly IResponseQueue _Queue;
        private readonly IProtocol _Protocol;
        private readonly ConcurrentDictionary<long, SoulProxy> _Souls;
        private readonly ISerializable _Serializer;
        private readonly IInternalSerializable _InternalSerializable;

        public SoulPropertyHandler(
            IResponseQueue queue,
            IProtocol protocol,
            ConcurrentDictionary<long, SoulProxy> souls,
            ISerializable serializer,
            IInternalSerializable internalSerializable)
        {
            _Queue = queue;
            _Protocol = protocol;
            _Souls = souls;
            _Serializer = serializer;
            _InternalSerializable = internalSerializable;
        }

        public void SetPropertyDone(long entityId, int propertyId)
        {
            if (_Souls.TryGetValue(entityId, out var soul))
            {
                soul.PropertyUpdateReset(propertyId);
            }
        }

        // 其他處理屬性的相關方法可以在這裡實現
    }

}

