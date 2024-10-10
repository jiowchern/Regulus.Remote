using System;
using System.Linq;

namespace Regulus.Remote
{
    internal class GhostEventMoveHandler : ClientExchangeable
    {

        private readonly long _Ghost;

        private readonly IProtocol _Protocol;
        
        
        IInternalSerializable _InternalSerializable;
        public GhostEventMoveHandler(long ghost, IProtocol protocol,  IInternalSerializable internal_serializable)
        {
            _InternalSerializable = internal_serializable;
            this._Ghost = ghost;
            _Protocol = protocol;                        
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
        

        internal void Add(System.Reflection.EventInfo info, long handler)
        {
            MemberMap map = _Protocol.GetMemberMap();


            Regulus.Remote.Packages.PackageAddEvent package = new Regulus.Remote.Packages.PackageAddEvent();
            
            package.Entity = _Ghost;
            package.Event = map.GetEvent(info);
            package.Handler = handler;

            _ResponseEvent(ClientToServerOpCode.AddEvent, _InternalSerializable.Serialize(package));

        }

       

        internal void Remove(System.Reflection.EventInfo info, long handler)
        {
            MemberMap map = _Protocol.GetMemberMap();
            

            Regulus.Remote.Packages.PackageRemoveEvent package = new Regulus.Remote.Packages.PackageRemoveEvent();


            package.Entity = _Ghost;
            package.Event = map.GetEvent(info);
            package.Handler = handler;

            _ResponseEvent(ClientToServerOpCode.RemoveEvent, _InternalSerializable.Serialize(package));

        }

        
    }
}