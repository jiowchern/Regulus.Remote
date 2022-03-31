using Regulus.Remote.Extensions;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote
{
    internal class GhostMethodHandler
    {
        
        private readonly IProtocol _Protocol;
        private readonly ISerializable _Serializable;
        private readonly ReturnValueQueue _ReturnValueQueue;

        private readonly long _Ghost;

        private readonly IOpCodeExchangeable _Requester;
        readonly IInternalSerializable _InternalSerializable;
        public GhostMethodHandler(long ghost, 
            ReturnValueQueue return_value_queue, 
            IProtocol protocol ,
            ISerializable serializable, 
            IInternalSerializable internal_serializable,
            Regulus.Remote.IOpCodeExchangeable requester)
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
            ISerializable serialize = _Serializable;
            int method = map.GetMethod(info);

            Regulus.Remote.Packages.PackageCallMethod package = new Regulus.Remote.Packages.PackageCallMethod();

            
                
            package.EntityId = _Ghost;
            package.MethodId = method;
            
            package.MethodParams = args.Zip(info.GetParameters(), (arg, par) => serialize.Serialize(par.ParameterType, arg)).ToArray();

            if (return_value != null)
                package.ReturnId = _ReturnValueQueue.PushReturnValue(return_value);
            
            _Requester.Request(ClientToServerOpCode.CallMethod, _InternalSerializable.Serialize(package));
        }
    }
}