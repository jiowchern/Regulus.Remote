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
        private readonly Regulus.Utility.Updater<Timestamp> _Updater;
        private readonly WiringOperator _WiringOperator;

        public Agent(IRecevieable recevieable, ISendable sendable)
        {
            _Recevieable = recevieable;
            _Sendable = sendable;
            _Updater = new Updater<Timestamp>();                    

            _WiringOperator = new WiringOperator(_Sendable, _Recevieable);
            

            _Peers = new Dictionary<EndPoint, Peer>();
        }       

        bool IUpdatable<Timestamp>.Update(Timestamp timestamp)
        {
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

        private void _DestroyPeer(ILine obj)
        {
            Peer peer;
            if (_Peers.TryGetValue(obj.EndPoint, out peer))
            {
                peer.Release();
                _Updater.Remove(peer);
                _Peers.Remove(obj.EndPoint);
            }
            
        }

        private void _CreatePeer(ILine obj)
        {            
        }

        public void Disconnect(EndPoint end_point)
        {
            Peer peer;
            if (_Peers.TryGetValue(end_point, out peer))
            {
                peer.Close();
            }
        }
        public IPeer Connect(EndPoint end_point)
        {
            IPeer peer = null;
            System.Action<ILine> handler = stream =>
            {
                var p = new Peer(stream, new PeerConnecter(stream));
                _Peers.Add(stream.EndPoint, p);
                _Updater.Add(p);
                peer = p;
            };
            _WiringOperator.JoinStreamEvent += handler;
            _WiringOperator.Create(end_point);
            _WiringOperator.JoinStreamEvent -= handler;

            
            return peer;
        }


        public static Agent CreateStandard()
        {            
            var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            return new Agent(new SocketRecevier(socket, Config.PackageSize), new SocketSender(socket));
        }
    }
}