using System;

namespace Regulus.Remote
{
    internal class GhostEventMoveHandler
    {
        private IGhost _Ghost;
        
        private IProtocol _Protocol;
        private IGhostRequest _Requester;

        public GhostEventMoveHandler(IGhost ghost, IProtocol protocol, IGhostRequest requester)
        {
            this._Ghost = ghost;            
            _Protocol = protocol;
            _Requester = requester;
        }

        internal void Add(System.Reflection.EventInfo info,long handler)
        {
            var map = _Protocol.GetMemberMap();
            var serialize = _Protocol.GetSerialize();

            var package = new PackageAddEvent();
            package.Entity = _Ghost.GetID();
            package.Event = map.GetEvent(info);
            package.Handler = handler;
            _Requester.Request(ClientToServerOpCode.AddEvent, package.ToBuffer(serialize));

        }

        internal void Remove(System.Reflection.EventInfo info, long handler)
        {
            var map = _Protocol.GetMemberMap();
            var serialize = _Protocol.GetSerialize();

            var package = new PackageRemoveEvent();
            package.Entity = _Ghost.GetID();
            package.Event = map.GetEvent(info);
            package.Handler = handler;
            _Requester.Request(ClientToServerOpCode.RemoveEvent, package.ToBuffer(serialize));

        }
    }
}