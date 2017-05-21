using System;
using System.Linq;
using System.Reflection;

namespace Regulus.Remoting
{
    internal class GhostMethodHandler
    {
        private readonly IProtocol _Protocol;

        private readonly ReturnValueQueue _ReturnValueQueue;

        private readonly IGhost _Ghost;

        private readonly IGhostRequest _Requester;

        public GhostMethodHandler(IGhost ghost, ReturnValueQueue return_value_queue, IProtocol protocol, Regulus.Remoting.IGhostRequest requester)
        {
            _Ghost = ghost;
            _ReturnValueQueue = return_value_queue;
            _Protocol = protocol;
            _Requester = requester;
        }

        public void Run(MethodInfo info, object[] args, IValue return_value)
        {
            var map = _Protocol.GetMemberMap();
            var serialize = _Protocol.GetSerialize();
            var method = map.GetMethod(info);
            
            var package = new PackageCallMethod();
            package.EntityId = _Ghost.GetID();
            package.MethodId = method;
            package.MethodParams = args.Select(arg => serialize.Serialize(arg)).ToArray();

            if (return_value != null)
                package.ReturnId = _ReturnValueQueue.PushReturnValue(return_value);

            _Requester.Request(ClientToServerOpCode.CallMethod, package.ToBuffer(serialize));
        }
    }
}