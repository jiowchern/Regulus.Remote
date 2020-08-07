namespace Regulus.Remote
{
    internal class GhostEventMoveHandler
    {
        private readonly IGhost _Ghost;

        private readonly IProtocol _Protocol;
        private readonly IGhostRequest _Requester;

        public GhostEventMoveHandler(IGhost ghost, IProtocol protocol, IGhostRequest requester)
        {
            this._Ghost = ghost;
            _Protocol = protocol;
            _Requester = requester;
        }

        internal void Add(System.Reflection.EventInfo info, long handler)
        {
            MemberMap map = _Protocol.GetMemberMap();
            Serialization.ISerializer serialize = _Protocol.GetSerialize();

            PackageAddEvent package = new PackageAddEvent();
            package.Entity = _Ghost.GetID();
            package.Event = map.GetEvent(info);
            package.Handler = handler;
            _Requester.Request(ClientToServerOpCode.AddEvent, package.ToBuffer(serialize));

        }

        internal void Remove(System.Reflection.EventInfo info, long handler)
        {
            MemberMap map = _Protocol.GetMemberMap();
            Serialization.ISerializer serialize = _Protocol.GetSerialize();

            PackageRemoveEvent package = new PackageRemoveEvent();
            package.Entity = _Ghost.GetID();
            package.Event = map.GetEvent(info);
            package.Handler = handler;
            _Requester.Request(ClientToServerOpCode.RemoveEvent, package.ToBuffer(serialize));

        }
    }
}