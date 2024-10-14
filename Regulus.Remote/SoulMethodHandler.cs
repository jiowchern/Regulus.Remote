using Regulus.Memorys;
using Regulus.Remote.Packages;
using Regulus.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote
{
    // MethodHandler.cs
    public class SoulMethodHandler
    {
        private readonly IRequestQueue _Peer;
        private readonly IResponseQueue _Queue;
        private readonly IProtocol _Protocol;
        private readonly ConcurrentDictionary<long, SoulProxy> _Souls;
        private readonly Dictionary<long, IValue> _WaitValues;
        private readonly ISerializable _Serializer;
        private readonly IInternalSerializable _InternalSerializable;

        public SoulMethodHandler(
            IRequestQueue peer,
            IResponseQueue queue,
            IProtocol protocol,
            ConcurrentDictionary<long, SoulProxy> souls,
            Dictionary<long, IValue> waitValues,
            ISerializable serializer,
            IInternalSerializable internalSerializable)
        {
            _Peer = peer;
            _Queue = queue;
            _Protocol = protocol;
            _Souls = souls;
            _WaitValues = waitValues;
            _Serializer = serializer;
            _InternalSerializable = internalSerializable;

            _Peer.InvokeMethodEvent += _InvokeMethod;
        }

        public void Dispose()
        {
            _Peer.InvokeMethodEvent -= _InvokeMethod;
        }

        private void _InvokeMethod(long entity_id, int method_id, long returnId, byte[][] args)
        {
            SoulProxy soul;
            if (!_Souls.TryGetValue(entity_id, out soul))
                return;

            var soulInfo = new
            {
                soul.MethodInfos,
                soul.ObjectInstance
            };

            MethodInfo info = _Protocol.GetMemberMap().GetMethod(method_id);
            MethodInfo methodInfo =
                (from m in soulInfo.MethodInfos where m == info && m.GetParameters().Count() == args.Count() select m)
                    .FirstOrDefault();
            if (methodInfo == null)
                return;
            try
            {

                IEnumerable<object> argObjects = args.Zip(methodInfo.GetParameters(), (arg, par) => _Serializer.Deserialize(par.ParameterType, arg.AsBuffer()));

                object returnValue = methodInfo.Invoke(soulInfo.ObjectInstance, argObjects.ToArray());
                if (returnValue != null)
                {
                    _ReturnValue(returnId, returnValue as IValue);
                }
            }
            catch (DeserializeException deserialize_exception)
            {
                string message = deserialize_exception.Base.ToString();
                _ErrorDeserialize(method_id.ToString(), returnId, message);
            }
            catch (Exception e)
            {
                Log.Instance.WriteDebug(e.ToString());
                _ErrorDeserialize(method_id.ToString(), returnId, e.Message);
            }
        }

        private void _ReturnValue(long returnId, IValue returnValue)
        {
            if (_WaitValues.ContainsKey(returnId))
                return;

            _WaitValues.Add(returnId, returnValue);
            returnValue.QueryValue(obj =>
            {
                if (!returnValue.IsInterface())
                {
                    _ReturnDataValue(returnId, returnValue);
                }
                else
                {
                    _ReturnSoulValue(returnId, returnValue);
                }
                _WaitValues.Remove(returnId);
            });
        }

        private void _ReturnDataValue(long returnId, IValue returnValue)
        {
            var value = returnValue.GetObject();
            var package = new PackageReturnValue
            {
                ReturnTarget = returnId,
                ReturnValue = _Serializer.Serialize(returnValue.GetObjectType(), value).ToArray()
            };
            _Queue.Push(ServerToClientOpCode.ReturnValue, _InternalSerializable.Serialize(package));
        }

        private void _ReturnSoulValue(long returnId, IValue returnValue)
        {
            // 需要引用到 BindHandler，這裡假設有一個方法可以完成這個功能
            // _bindHandler.Bind(returnValue.GetObject(), returnValue.GetObjectType(), true, returnId);
        }

        private void _ErrorDeserialize(string methodName, long returnId, string message)
        {
            var package = new PackageErrorMethod
            {
                Method = methodName,
                ReturnTarget = returnId,
                Message = message
            };
            _Queue.Push(ServerToClientOpCode.ErrorMethod, _InternalSerializable.Serialize(package));
        }
    }

}
