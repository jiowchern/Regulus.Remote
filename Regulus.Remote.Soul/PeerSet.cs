using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Regulus.Utility;

namespace Regulus.Remote.Soul
{
	public class PeerSet
	{
		private readonly List<Peer> _Peers;

		public int Count
		{
			get { return _Peers.Count; }
		}

		public PeerSet()
		{
			_Peers = new List<Peer>();
		}

		internal void Join(Peer peer)
		{
			_Join(peer, peer);
		}

		internal void _Join(Peer peer, IBootable bootable)
		{
		    
		    
		    _Peers.Add(peer);		            
            
			bootable.Launch();
        }

		internal void _Leave(Peer peer, IBootable bootable)
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
			var removes = new System.Collections.Generic.List<Peer>();
			foreach(var peer in _Peers)
            {
				if (!peer.Connecting())
				{
					removes.Add(peer);
				}
			}			

			foreach(var r in removes)
            {
				_Leave(r, r);
			}
		}

        
    }
}
 