using System;
using System.Reflection;

namespace Regulus.Remote
{
    internal class GhostNotifierHandler
    {
        private IGhost _Ghost;
        private IProtocol _Protocol;
        private IGhostRequest _Requester;
        private SoulNotifier _NotifierPassage;

        public GhostNotifierHandler(IGhost ghost, IProtocol protocol, IGhostRequest requester, SoulNotifier notifierPassage)
        {
            this._Ghost = ghost;
            _Protocol = protocol;
            _Requester = requester;
            _NotifierPassage = notifierPassage;
        }

        internal void AddSupply(PropertyInfo info, PassageCallback passage)
        {
            var passageId = _NotifierPassage.RegisterSupply(passage);

            var package = new PackageNotifierEvent();
            package.Entity = _Ghost.GetID();
            package.Property = _Protocol.GetMemberMap().GetProperty(info);
            package.Passage = passageId;
            _Requester.Request(ClientToServerOpCode.AddNotifierSupply, package.ToBuffer(_Protocol.GetSerialize()));
        }

        internal void RemoveSupply(PropertyInfo info, PassageCallback passage)
        {
            var passageId = _NotifierPassage.UnregisterSupply(passage);

            var package = new PackageNotifierEvent();
            package.Entity = _Ghost.GetID();
            package.Property = _Protocol.GetMemberMap().GetProperty(info);
            package.Passage = passageId;
            _Requester.Request(ClientToServerOpCode.RemoveNotifierSupply, package.ToBuffer(_Protocol.GetSerialize()));
        }

        internal void AddUnsupply(PropertyInfo info, PassageCallback passage)
        {
            var passageId = _NotifierPassage.RegisterUnsupply(passage);

            var package = new PackageNotifierEvent();
            package.Entity = _Ghost.GetID();
            package.Property = _Protocol.GetMemberMap().GetProperty(info);
            package.Passage = passageId;
            _Requester.Request(ClientToServerOpCode.AddNotifierUnsupply, package.ToBuffer(_Protocol.GetSerialize()));
        }

        internal void RemoveUnsupply(PropertyInfo info, PassageCallback passage)
        {
            var passageId = _NotifierPassage.UnregisterUnsupply(passage);

            var package = new PackageNotifierEvent();
            package.Entity = _Ghost.GetID();
            package.Property = _Protocol.GetMemberMap().GetProperty(info);
            package.Passage = passageId;
            _Requester.Request(ClientToServerOpCode.RemoveNotifierUnsupply, package.ToBuffer(_Protocol.GetSerialize()));
        }
    }
}