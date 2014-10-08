using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Reflection.Emit;


namespace Regulus.Remoting.Ghost
{
    namespace Photon
    {
        public class Agent : ExitGames.Client.Photon.IPhotonPeerListener , IAgent
        {
            Regulus.Remoting.PhotonExtension.ClientPeer _Peer;
            LinkState _LinkState;
            Config _Config;
            Regulus.Remoting.AgentCore _Core;
            public Agent(Config config)
            {
                _Config = config;

            }

            public void Launch()
            {
                if (_Peer != null)
                    _Peer.Disconnect();
                _Peer = new Regulus.Remoting.PhotonExtension.ClientPeer(this, ExitGames.Client.Photon.ConnectionProtocol.Udp);
                _Peer.Connect(_Config.Address, _Config.Name);
                _Core = new AgentCore(_Peer);
            }
            public void Launch(LinkState link_state)
            {
                _LinkState = link_state;
                Launch();
            }

            public bool Update()
            {
                if (_Peer != null)
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
                get { return _Core.Ping; }
            }

            public Regulus.Remoting.Ghost.IProviderNotice<T> QueryProvider<T>()
            {
                return _Core.QueryProvider<T>();
            }
            void ExitGames.Client.Photon.IPhotonPeerListener.OnOperationResponse(ExitGames.Client.Photon.OperationResponse operationResponse)
            {
                var args = new Dictionary<byte, byte[]>();
                foreach(var p in operationResponse.Parameters)
                {
                    args.Add(p.Key , p.Value as byte[]);
                }
                _Core.OnResponse(operationResponse.OperationCode, args);
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

                    if (_Core != null)
                        _Core.Finial();
                    if (_LinkState.LinkFail != null)
                        _LinkState.LinkFail(statusCode.ToString() + ":" + ((_Peer != null) ? _Peer.PeerState.ToString() : "peer == null"));
                    _Peer = null;
                }

                //System.Diagnostics.Debug.WriteLine(statusCode.ToString());
            }



            IProviderNotice<T> IAgent.QueryProvider<T>()
            {
                throw new NotImplementedException();
            }

            Value<bool> IAgent.Connect(string account, int password)
            {
                throw new NotImplementedException();
            }


            long IAgent.Ping
            {
                get
                {
                    throw new NotImplementedException();
                }
                
            }

            event Action IAgent.DisconnectEvent
            {
                add { throw new NotImplementedException(); }
                remove { throw new NotImplementedException(); }
            }

            void IAgent.Disconnect()
            {
                throw new NotImplementedException();
            }

            bool Utility.IUpdatable.Update()
            {
                throw new NotImplementedException();
            }

            void Framework.ILaunched.Launch()
            {
                throw new NotImplementedException();
            }

            void Framework.ILaunched.Shutdown()
            {
                throw new NotImplementedException();
            }


            event Action IAgent.ConnectEvent
            {
                add { throw new NotImplementedException(); }
                remove { throw new NotImplementedException(); }
            }
        }
    }
	
}
