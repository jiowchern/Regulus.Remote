using Regulus.Remote.Extensions;
using Regulus.Remote.ProviderHelper;
using System;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote
{
    internal class GhostMethodHandler : ClientExchangeable
    {
        
        private readonly IProtocol _Protocol;
        private readonly ISerializable _Serializable;
        private readonly GhostsReturnValueHandler _ReturnValueQueue;

        private readonly long _Ghost;

        
        readonly IInternalSerializable _InternalSerializable;
        private readonly ServerToClientOpCode[] _Empty;

        public GhostMethodHandler(long ghost,
            GhostsReturnValueHandler return_value_queue, 
            IProtocol protocol ,
            ISerializable serializable, 
            IInternalSerializable internal_serializable)
        {
            _InternalSerializable = internal_serializable;
            _Ghost = ghost;
            _ReturnValueQueue = return_value_queue;
            _Protocol = protocol;
            this._Serializable = serializable;
            _Empty = new ServerToClientOpCode[0];
        }
        void Exchangeable<ServerToClientOpCode, ClientToServerOpCode>.Request(ServerToClientOpCode code, Memorys.Buffer args)
        {            
        }
        event Action<ClientToServerOpCode, Memorys.Buffer> _ResponseEvent;
        event Action<ClientToServerOpCode, Memorys.Buffer> Exchangeable<ServerToClientOpCode, ClientToServerOpCode>.ResponseEvent
        {
            add
            {
                _ResponseEvent += value;
            }

            remove
            {
                _ResponseEvent -= value;
            }
        }

        public void Run(MethodInfo info, object[] args, IValue return_value)
        {
            MemberMap map = _Protocol.GetMemberMap();
            ISerializable serialize = _Serializable;
            int method = map.GetMethod(info);

            Regulus.Remote.Packages.PackageCallMethod package = new Regulus.Remote.Packages.PackageCallMethod();

            
                
            package.EntityId = _Ghost;
            package.MethodId = method;
            
            package.MethodParams = args.Zip(info.GetParameters(), (arg, par) => serialize.Serialize(par.ParameterType, arg).ToArray()).ToArray();

            
            if (return_value != null)
                package.ReturnId = _ReturnValueQueue.PushReturnValue(return_value);

            _ResponseEvent(ClientToServerOpCode.CallMethod, _InternalSerializable.Serialize(package));            
        }

       
    }
}