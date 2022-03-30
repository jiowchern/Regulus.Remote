namespace Regulus.Remote
{
    internal class GhostEventMoveHandler
    {

        private readonly long _Ghost;

        private readonly IProtocol _Protocol;
        
        private readonly IOpCodeExchangeable _Requester;
        IInternalSerializable _InternalSerializable;
        public GhostEventMoveHandler(long ghost, IProtocol protocol,  IInternalSerializable internal_serializable, IOpCodeExchangeable requester)
        {
            _InternalSerializable = internal_serializable;
            this._Ghost = ghost;
            _Protocol = protocol;            
            _Requester = requester;
        }

        internal void Add(System.Reflection.EventInfo info, long handler)
        {
            MemberMap map = _Protocol.GetMemberMap();


            Regulus.Remote.Packages.PackageAddEvent package = new Regulus.Remote.Packages.PackageAddEvent();
            
            package.Entity = _Ghost;
            package.Event = map.GetEvent(info);
            package.Handler = handler;

            _Requester.Request(ClientToServerOpCode.AddEvent, _InternalSerializable.Serialize(package));

        }

       

        internal void Remove(System.Reflection.EventInfo info, long handler)
        {
            MemberMap map = _Protocol.GetMemberMap();
            

            Regulus.Remote.Packages.PackageRemoveEvent package = new Regulus.Remote.Packages.PackageRemoveEvent();


            package.Entity = _Ghost;
            package.Event = map.GetEvent(info);
            package.Handler = handler;
            
            _Requester.Request(ClientToServerOpCode.RemoveEvent, _InternalSerializable.Serialize(package));

        }
    }
}