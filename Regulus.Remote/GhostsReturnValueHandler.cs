using Regulus.Memorys;
using System;

namespace Regulus.Remote
{
    namespace ProviderHelper
    {
        public class GhostsReturnValueHandler
        {
            private readonly ReturnValueQueue _ReturnValueQueue;
            private readonly ISerializable _Serializer;

            public event Action<string, string> ErrorMethodEvent;

            public GhostsReturnValueHandler(ISerializable serializer)
            {
                _ReturnValueQueue = new ReturnValueQueue();
                _Serializer = serializer;
            }

            public void SetReturnValue(long returnTarget, byte[] returnValue)
            {
                IValue value = _ReturnValueQueue.PopReturnValue(returnTarget);
                if (value != null)
                {
                    object returnInstance = _Serializer.Deserialize(value.GetObjectType(), returnValue.AsBuffer());
                    value.SetValue(returnInstance);
                }
            }

            public void ErrorReturnValue(long returnTarget, string method, string message)
            {
                _ReturnValueQueue.PopReturnValue(returnTarget);
                ErrorMethodEvent?.Invoke(method, message);
            }
            public void PopReturnValue(long return_id, IGhost ghost)
            {
                IValue value = _ReturnValueQueue.PopReturnValue(return_id);
                if (value != null)
                {
                    value.SetValue(ghost);
                }
            }
            

            public IValue GetReturnValue(long returnId)
            {
                return _ReturnValueQueue.PopReturnValue(returnId);
            }

            internal long PushReturnValue(IValue return_value)
            {
                return _ReturnValueQueue.PushReturnValue(return_value);
            }
        }

    }
}
