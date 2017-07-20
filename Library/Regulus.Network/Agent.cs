using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    public class Agent : IUpdatable<Timestamp>
    {
        
        private readonly IRecevieable _Recevieable;
        private readonly ISendable _Sendable;


        private readonly Dictionary<EndPoint, Peer> _Peers;
        private readonly List<Peer> _RemovePeers;
        private readonly Regulus.Utility.Updater<Timestamp> _Updater;
        private readonly WiringOperator _WiringOperator;

        public Agent(IRecevieable recevieable, ISendable sendable)
        {
            _Recevieable = recevieable;
            _Sendable = sendable;
            _Updater = new Updater<Timestamp>();
            _RemovePeers = new List<Peer>();
            _WiringOperator = new WiringOperator(_Sendable, _Recevieable);
            

            _Peers = new Dictionary<EndPoint, Peer>();
        }       

        bool IUpdatable<Timestamp>.Update(Timestamp timestamp)
        {
            var count = _RemovePeers.Count;
            for (int i = 0; i < count; i++)
            {
                var peer = _RemovePeers[i];
                _WiringOperator.Destroy(peer.EndPoint);                
            }
            _RemovePeers.Clear();


            _Updater.Working(timestamp);
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

        private void _DestroyPeer(Line obj)
        {
            Peer peer;
            if (_Peers.TryGetValue(obj.EndPoint, out peer))
            {
                peer.Break();
                _Updater.Remove(peer);
                _Peers.Remove(obj.EndPoint);
            }
            
        }

        private void _CreatePeer(Line obj)
        {            
        }
        
        public IPeer Connect(EndPoint end_point,System.Action<bool> result)
        {
            IPeer peer = null;
            System.Action<Line> handler = stream =>
            {
                var connecter = new PeerConnecter(stream);
                connecter.TimeoutEvent += () => { result(false); };
                connecter.DoneEvent += () => { result(true); };

                var p = new Peer(stream, connecter);
                p.CloseEvent += () => { _Remove(p); };
                _Peers.Add(stream.EndPoint, p);
                _Updater.Add(p);
                peer = p;
            };
            _WiringOperator.JoinStreamEvent += handler;
            _WiringOperator.Create(end_point);
            _WiringOperator.JoinStreamEvent -= handler;

            
            return peer;
        }

        private void _Remove(Peer peer)
        {
            _RemovePeers.Add(peer);


        }


        public static Agent CreateStandard()
        {            
            var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            return new Agent(new SocketRecevier(socket), new SocketSender(socket));
        }
    }
}