using System;
using System.Collections.Generic;


using Regulus.Framework;

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
		    lock (_Peers)
		    {
		        _Peers.Add(peer);
		        peer.DisconnectEvent += () => { _Leave(peer, peer); };		        
            }
		    bootable.Launch();
        }

		internal void _Leave(Peer peer, IBootable bootable)
		{
			lock(_Peers)
			{
			    if (_Peers.Remove(peer))
			    {
			        bootable.Shutdown();
			    }
			    else
			    {
                    if(_Peers.Count > 0)
			            throw new Exception("no peer shutdown.");
			    }
			}
		}

		internal void Release()
		{
			lock(_Peers)
			{
				foreach(IBootable peer in _Peers)
				{
					peer.Shutdown();
				}

				_Peers.Clear();
			}
		}
	}
}
