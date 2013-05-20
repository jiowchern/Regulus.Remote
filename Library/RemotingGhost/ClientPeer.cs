using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting.PhotonExtension
{
	class ClientPeer : ExitGames.Client.Photon.PhotonPeer
	{
		///////////////////////////////////////////////////////////////////////////////////////
		///
		///////////////////////////////////////////////////////////////////////////////////////
		public ClientPeer(ExitGames.Client.Photon.IPhotonPeerListener listener, ExitGames.Client.Photon.ConnectionProtocol protocolType)
			: base(listener, protocolType)
		{

		}

		///////////////////////////////////////////////////////////////////////////////////////
		/// 斷線的廣播事件
		///////////////////////////////////////////////////////////////////////////////////////
		public event Action DisconnectEvent;
		public override void Disconnect()
		{
			if (DisconnectEvent != null)
				DisconnectEvent.Invoke();
			base.Disconnect();
		}
	}
}
