using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Regulus.Serialization;

namespace Regulus.Network.RUDP
{
	public class Host 
	{
		public event System.Action<Peer> AcceptEvent;		
		private readonly Socket _Socket;
		private byte[] _Buffer;
		private int _Offset;	    
	    private ReceiveHandler _ReceiveHandler;
	    private Serializer _Serializer;

	    private readonly List<Peer> _Peers;
	    public Host(int port)
		{
		    _Peers = new List<Peer>();
		    _Serializer = Peer.CreateSerializer(); 
			_Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
			_Socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, port));		    

		    _ReceiveHandler = new ReceiveHandler(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0) , _Socket , _Serializer);
		    _ReceiveHandler.DataPackageEvent += _EmptyData;
		    _ReceiveHandler.AckPackageEvent += _EmptyAck;
		    _ReceiveHandler.PingPackageEvent += _EmptyPing;
		    _ReceiveHandler.ConnectPackageEvent += _ProcessConnect;
            _ReceiveHandler.Start();

            
		}

	    private void _ProcessConnect(ref ConnectPackage package, ref IPEndPoint address)
        {
            if (_HavePeer(address) == false)
            {
                _CreatePeer(ref address);
            }
        }

	    private void _CreatePeer(ref IPEndPoint address)
	    {
	        var peer = new Peer(ref address , _Socket);
            _Peers.Add(peer);
	        AcceptEvent(peer);
	    }

	    private bool _HavePeer(IPEndPoint address)
        {
            for (int i = 0; i < _Peers.Count; i++)
            {
                var peer = _Peers[i];
                if (Equals(peer.Address, address))
                {
                    return true;
                }
            }

            return false;
        }

        private void _EmptyPing(ref PingPackage package, ref IPEndPoint address)
	    {	        
	    }

	    private void _EmptyAck(ref AckPackage package, ref IPEndPoint address)
	    {	        
	    }

	    private void _EmptyData(ref MessagePackage package, ref IPEndPoint address)
	    {	        
	    }
	}
}
