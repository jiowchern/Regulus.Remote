using Regulus.Utility;
using System;
using System.Collections.Generic;

namespace Regulus.Remote.Soul
{
    public class PeerSet
    {
        private readonly List<User> _Peers;

        public int Count
        {
            get { return _Peers.Count; }
        }

        public PeerSet()
        {
            _Peers = new List<User>();
        }

        internal void Join(User peer)
        {
            //todo : _Join(peer, peer);
        }

        internal void _Join(User peer, IBootable bootable)
        {


            _Peers.Add(peer);

            bootable.Launch();
        }

        internal void _Leave(User peer, IBootable bootable)
        {
            if (_Peers.Remove(peer))
            {
                bootable.Shutdown();
            }
            else
            {
                if (_Peers.Count > 0)
                    throw new Exception("no peer shutdown.");
            }
        }

        internal void Release()
        {
            foreach (IBootable peer in _Peers)
            {
                peer.Shutdown();
            }

            _Peers.Clear();
        }
        public void RemoveInvalidPeers()
        {
            List<User> removes = new System.Collections.Generic.List<User>();
            foreach (User peer in _Peers)
            {
                /*todo :if (!peer.Connecting())
				{
					removes.Add(peer);
				}*/
            }

            foreach (User r in removes)
            {
                //todo : _Leave(r, r);
            }
        }


    }
}
