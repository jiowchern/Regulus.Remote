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
        
        private readonly ISocketRecevieable _SocketRecevieable;
        private readonly ISocketSendable _SocketSendable;


        private readonly Dictionary<EndPoint, Peer> _Peers;
        private readonly List<Peer> _RemovePeers;
        private readonly Regulus.Utility.Updater<Timestamp> _Updater;
        private readonly WiringOperator _WiringOperator;

        public Agent(ISocketRecevieable socket_recevieable, ISocketSendable socket_sendable)
        {
            _SocketRecevieable = socket_recevieable;
            _SocketSendable = socket_sendable;            
            _Updater = new Updater<Timestamp>();
            _RemovePeers = new List<Peer>();
            _WiringOperator = new WiringOperator(_SocketSendable, _SocketRecevieable);            
            _Peers = new Dictionary<EndPoint, Peer>();
        }       

        bool IUpdatable<Timestamp>.Update(Timestamp timestamp)
        {
            var removePeers = _GetRemovePeers();
            var count = removePeers.Length;
            for (int i = 0; i < count; i++)
            {
                var peer = removePeers[i];
                lock (_WiringOperator)
                {
                    _WiringOperator.Destroy(peer.EndPoint);
                }                
            }

            _Updater.Working(timestamp);
            return true;
        }

        private Peer[] _GetRemovePeers()
        {
            lock (_RemovePeers)
            {
                var cloned = _RemovePeers.ToArray();
                _RemovePeers.Clear();
                return cloned;
            }
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
                lock (_Peers)
                {
                    _Peers.Remove(obj.EndPoint);
                }
                
            }
            
        }

        private void _CreatePeer(Line obj)
        {            
        }
        
        public IRudpPeer Connect(EndPoint end_point,System.Action<bool> result)
        {
            IRudpPeer rudpPeer = null;
            System.Action<Line> handler = stream =>
            {
                var connecter = new PeerConnecter(stream);
                connecter.TimeoutEvent += () => { result(false); };
                connecter.DoneEvent += () => { result(true); };

                var p = new Peer(stream, connecter);
                p.CloseEvent += () => { _Remove(p); };

                lock (_Peers)
                {
                    _Peers.Add(stream.EndPoint, p);
                }
                
                _Updater.Add(p);
                rudpPeer = p;
            };

            lock (_WiringOperator)
            {
                _WiringOperator.JoinStreamEvent += handler;
                _WiringOperator.Create(end_point);
                _WiringOperator.JoinStreamEvent -= handler;
            }
            
            return rudpPeer;
        }

        private void _Remove(Peer peer)
        {
            lock (_RemovePeers)
            {
                _RemovePeers.Add(peer);
            }

        }


        
        
    }
}