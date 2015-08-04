// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeerSet.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the PeerSet type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

using Regulus.Framework;

#endregion

namespace Regulus.Remoting.Soul.Native
{
	public class PeerSet
	{
		private readonly List<Peer> _Peers;

		public int Count
		{
			get { return this._Peers.Count; }
		}

		public PeerSet()
		{
			this._Peers = new List<Peer>();
		}

		internal void Join(Peer peer)
		{
			this._Join(peer, peer);
		}

		internal void _Join(Peer peer, IBootable bootable)
		{
			lock (this._Peers)
				this._Peers.Add(peer);
			peer.DisconnectEvent += () => { this._Leave(peer, peer); };
			bootable.Launch();
		}

		internal void _Leave(Peer peer, IBootable bootable)
		{
			lock (this._Peers)
			{
				if (this._Peers.Remove(peer))
				{
					bootable.Shutdown();
				}
			}
		}

		internal void Release()
		{
			lock (this._Peers)
			{
				foreach (IBootable peer in this._Peers)
				{
					peer.Shutdown();
				}

				this._Peers.Clear();
			}
		}
	}
}