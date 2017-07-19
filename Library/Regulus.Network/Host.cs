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
	    private readonly WiringOperator _WiringOperator;
        public event System.Action<IPeer> AcceptEvent;

	    private readonly Dictionary<ILine , Peer> _Peers;
	    private readonly List<Peer> _RemovePeers;
        private readonly Regulus.Utility.Updater<Timestamp> _Updater;
	    
        public int PeerCount { get { return _Peers.Count; } }
	    public static Host CreateStandard(int port)
	    {
	        var endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, port);
	        var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
	        socket.Bind(endPoint);
            return new Host(new SocketRecevier(socket ),new SocketSender(socket) );
	    }

        public Host(IRecevieable recevieable , ISendable sendable)
        {
            _RemovePeers = new List<Peer>();
            _Updater = new Updater<Timestamp>();
            _Peers = new Dictionary<ILine, Peer>();
            _WiringOperator = new WiringOperator(sendable, recevieable);
        }

	    


	    bool IUpdatable<Timestamp>.Update(Timestamp arg)
	    {
	        _Updater.Working(arg);

	        var count = _RemovePeers.Count;
	        for (int index = 0; index < count; index++)
	        {
                _WiringOperator.Destroy(_RemovePeers[index].EndPoint);	            
	        }
	        _RemovePeers.Clear();
            return true;
	    }

	    

	    void IBootable.Launch()
	    {
	        _WiringOperator.JoinStreamEvent += _CreatePeer;
	        _WiringOperator.LeftStreamEvent += _DestroyPeer;

            _Updater.Add(_WiringOperator);

        }

	    void IBootable.Shutdown()
	    {
	        _Updater.Shutdown();
	        _WiringOperator.JoinStreamEvent -= _CreatePeer;
	        _WiringOperator.LeftStreamEvent -= _DestroyPeer;
        }

	    private void _DestroyPeer(ILine line)
	    {

	        var peer = _Peers.FirstOrDefault(p => p.Key == line);
	        

            if (peer.Key != null)
	        {
	            peer.Value.Break();
                _Peers.Remove(peer.Key);
	            _Updater.Remove(peer.Value);
            }
        }

	    

        private void _CreatePeer(ILine line)
        {
            var listener = new PeerListener(line);

            var peer = new Peer(line, listener);
            peer.CloseEvent += () => { _Remove(peer); };
            listener.DoneEvent += () =>
            {
                if (AcceptEvent != null)
                    AcceptEvent(peer);
            };
            

            _Peers.Add(line,peer);            
            _Updater.Add(peer);

	        
	    }

	    private void _Remove(Peer peer)
	    {
	        _RemovePeers.Add(peer);
        }
	}
}
