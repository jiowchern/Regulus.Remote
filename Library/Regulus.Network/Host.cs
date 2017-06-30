using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Regulus.Framework;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
	public class Host : IUpdatable<Timestamp>
	{
	    private readonly StreamProvider _StreamProvider;
        public event System.Action<IPeer> AcceptEvent;

	    private readonly Dictionary<IStream , Peer> _Peers;
	    private readonly Regulus.Utility.Updater<Timestamp> _Updater;
	    private Serializer _Serializer;

	    public static Host CreateStandard(int port)
	    {
	        var endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, port);
	        var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
	        socket.Bind(endPoint);
            return new Host(new SocketRecevier(socket , Config.PackageSize ),new SocketSender(socket) );
	    }

        public Host(IRecevieable recevieable , ISendable sendable)
        {
            
            _Updater = new Updater<Timestamp>();
            _Peers = new Dictionary<IStream, Peer>();
            _StreamProvider = new StreamProvider(sendable, recevieable);
        }

	    


	    bool IUpdatable<Timestamp>.Update(Timestamp arg)
	    {
	        _Updater.Working(arg);
	        return true;
	    }

	    void IBootable.Launch()
	    {
	        _StreamProvider.JoinStreamEvent += _AddPeer;
	        _StreamProvider.LeftStreamEvent += _RemovePeer;

            _Updater.Add(_StreamProvider);

        }

	    void IBootable.Shutdown()
	    {
	        _Updater.Shutdown();
	        _StreamProvider.JoinStreamEvent -= _AddPeer;
	        _StreamProvider.LeftStreamEvent -= _RemovePeer;
        }

	    private void _RemovePeer(IStream stream)
	    {

	        var peer = _Peers.FirstOrDefault(p => p.Key == stream);
	        

            if (peer.Key != null)
	        {
	            _Peers.Remove(peer.Key);
	            _Updater.Remove(peer.Value);
            }
        }

	    private void _AddPeer(IStream stream)
	    {
	        var peer = new Peer(stream , new PeerListener(stream));
	        _Peers.Add(stream,peer);            
            _Updater.Add(peer);

	        if (AcceptEvent != null)
	            AcceptEvent(peer);
	    }
	}
}
