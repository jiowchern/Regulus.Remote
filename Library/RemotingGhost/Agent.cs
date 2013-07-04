using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Reflection.Emit;


namespace Regulus.Remoting.Ghost
{
	public class Agent :  ExitGames.Client.Photon.IPhotonPeerListener
	{
		Regulus.Remoting.PhotonExtension.ClientPeer _Peer;	
		LinkState							_LinkState;
		Config								_Config;
		Regulus.Remoting.AgentCore			_Core;
		public Agent(Config config) 
		{			
			_Config = config;
			_Core = new AgentCore(_Peer);
		}

        public void Launch()
        {
            if (_Peer != null)
                _Peer.Disconnect();
            _Peer = new Regulus.Remoting.PhotonExtension.ClientPeer(this, ExitGames.Client.Photon.ConnectionProtocol.Udp);
            _Peer.Connect(_Config.Address, _Config.Name);	
        }
		public void Launch(LinkState link_state)
		{			
			_LinkState = link_state;
            Launch();
		}

		public bool Update()
		{
            if (_Peer != null )
			{
				_Peer.Service();
				return true;
			}
			return false;				
		}

		public void Shutdown()
		{
            if (_Peer != null)
            {
                _Peer.Disconnect();                
                _Peer = null;                
            }			
		}

		public long Ping 
		{
			get { return _Core.Ping ;}
		}

		public Regulus.Remoting.Ghost.IProviderNotice<T> QueryProvider<T>()
		{
			return _Core.QueryProvider<T>();
		}
		void ExitGames.Client.Photon.IPhotonPeerListener.OnOperationResponse(ExitGames.Client.Photon.OperationResponse operationResponse)
		{
			_Core.OnResponse(operationResponse.OperationCode, operationResponse.Parameters);
		}
		void ExitGames.Client.Photon.IPhotonPeerListener.DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
		{
            System.Diagnostics.Debug.WriteLine("photon debgu return .level:" + level.ToString() + " message" + message);		
		}

		void ExitGames.Client.Photon.IPhotonPeerListener.OnEvent(ExitGames.Client.Photon.EventData eventData)
		{

		}
		void ExitGames.Client.Photon.IPhotonPeerListener.OnStatusChanged(ExitGames.Client.Photon.StatusCode statusCode)
		{
			if (statusCode == ExitGames.Client.Photon.StatusCode.Connect)
			{
				if (_LinkState.LinkSuccess != null)
					_LinkState.LinkSuccess.Invoke();
				_Core.Initial();
			}
			else if (statusCode == ExitGames.Client.Photon.StatusCode.QueueOutgoingReliableWarning)
			{
			}
			else if (statusCode == ExitGames.Client.Photon.StatusCode.QueueIncomingReliableWarning)
			{
			}
			else if (statusCode == ExitGames.Client.Photon.StatusCode.QueueOutgoingAcksWarning)
			{
			}
			else if (statusCode == ExitGames.Client.Photon.StatusCode.QueueSentWarning)
			{
			}
			else if (statusCode == ExitGames.Client.Photon.StatusCode.SendError)
			{

			}
			else
			{

				_Core.Finial();
				if (_LinkState.LinkFail != null)
					_LinkState.LinkFail(statusCode.ToString());
				_Peer = null;
			}

			System.Diagnostics.Debug.WriteLine(statusCode.ToString());
		}

        
	}
}
