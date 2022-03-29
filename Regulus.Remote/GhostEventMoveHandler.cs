namespace Regulus.Remote
{
    internal class GhostEventMoveHandler
    {

        private readonly System.WeakReference<IGhost> _Ghost;

        private readonly IProtocol _Protocol;
        
        private readonly IGhostRequest _Requester;
        IInternalSerializable _InternalSerializable;
        public GhostEventMoveHandler(System.WeakReference<IGhost> ghost, IProtocol protocol,  IInternalSerializable internal_serializable, IGhostRequest requester)
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
            IGhost ghost;
            if (!_FindGhost(info, out ghost))
                return;
            package.Entity = ghost.GetID();
            package.Event = map.GetEvent(info);
            package.Handler = handler;

            _Requester.Request(ClientToServerOpCode.AddEvent, _InternalSerializable.Serialize(package));

        }

        private bool _FindGhost(System.Reflection.EventInfo info, out IGhost ghost)
        {
            if (!_Ghost.TryGetTarget(out ghost))
            {
                Regulus.Utility.Log.Instance.WriteInfo($"The ghost of the {info} is no longer there.");
                return false;
            }
            return true;
        }

        internal void Remove(System.Reflection.EventInfo info, long handler)
        {
            MemberMap map = _Protocol.GetMemberMap();
            

            Regulus.Remote.Packages.PackageRemoveEvent package = new Regulus.Remote.Packages.PackageRemoveEvent();

            IGhost ghost;
            if (!_FindGhost(info, out ghost))
                return;

            package.Entity = ghost.GetID();
            package.Event = map.GetEvent(info);
            package.Handler = handler;
            
            _Requester.Request(ClientToServerOpCode.RemoveEvent, _InternalSerializable.Serialize(package));

        }
    }
}