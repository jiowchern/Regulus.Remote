using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Native.Soul
{
    public class PeerSet
    {
        List<Regulus.Remoting.Soul.Native.Peer> _Peers;

        public PeerSet()
        {
            _Peers = new List<Remoting.Soul.Native.Peer>();
        }
        internal void Join(Remoting.Soul.Native.Peer peer)
        {
            _Join(peer, peer);
        }

        internal void _Join(Remoting.Soul.Native.Peer peer , Regulus.Framework.IBootable bootable)
        {
            lock (_Peers)
                _Peers.Add(peer);
            peer.DisconnectEvent += () => { _Leave(peer, peer); };
            bootable.Launch();
        }

        internal void _Leave(Remoting.Soul.Native.Peer peer, Regulus.Framework.IBootable bootable)
        {            
            lock (_Peers)
            {
                if(_Peers.Remove(peer))
                {
                    bootable.Shutdown();
                }
            }
                
        }

        

        internal void Release()
        {
            lock (_Peers)
            {
                foreach (Regulus.Framework.IBootable peer in _Peers)
                {
                    peer.Shutdown();
                }
                _Peers.Clear();
            }
            
        }

        public int Count { get { return _Peers.Count; } }
    }
}
