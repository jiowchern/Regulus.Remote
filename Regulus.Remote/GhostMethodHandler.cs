using Regulus.Serialization;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote
{
    internal class GhostMethodHandler
    {
        private readonly IProtocol _Protocol;
        private readonly ISerializable _Serializable;
        private readonly ReturnValueQueue _ReturnValueQueue;

        private readonly IGhost _Ghost;

        private readonly IGhostRequest _Requester;
        readonly IInternalSerializable _InternalSerializable;
        public GhostMethodHandler(IGhost ghost, 
            ReturnValueQueue return_value_queue, 
            IProtocol protocol , 
            Serialization.ISerializable serializable, 
            IInternalSerializable internal_serializable,
            Regulus.Remote.IGhostRequest requester)
        {
            _InternalSerializable = internal_serializable;
            _Ghost = ghost;
            _ReturnValueQueue = return_value_queue;
            _Protocol = protocol;
            this._Serializable = serializable;
            _Requester = requester;
        }

        public void Run(MethodInfo info, object[] args, IValue return_value)
        {
            MemberMap map = _Protocol.GetMemberMap();
            Serialization.ISerializable serialize = _Serializable;
            int method = map.GetMethod(info);

            PackageCallMethod package = new PackageCallMethod();
            package.EntityId = _Ghost.GetID();
            package.MethodId = method;
            
            package.MethodParams = args.Zip(info.GetParameters(), (arg, par) => serialize.Serialize(par.ParameterType, arg)).ToArray();

            if (return_value != null)
                package.ReturnId = _ReturnValueQueue.PushReturnValue(return_value);

            _Requester.Request(ClientToServerOpCode.CallMethod, package.ToBuffer(_InternalSerializable));
        }
    }
}