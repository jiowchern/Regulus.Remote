using Regulus.Network;
using Regulus.Network.Tcp;
using Regulus.Remote.Soul;
using System;

namespace Regulus.Remote.Server.Tcp
{
    public class Listener
    {
        private readonly IService _Service;
        readonly Regulus.Network.Tcp.Listener _Listener;
        
        public Listener(IService service)
        {
            this._Service = service;
            
            _Listener = new Network.Tcp.Listener();

            _Listener.AcceptEvent += _Join;

        }
        public void Bind(int port)
        {
            _Listener.Bind(port);
        }

        public void Close() {
            _Listener.Close();
        }

        private void _Join(Peer peer)
        {
            peer.SocketErrorEvent += (e) => {
                _Service.Leave(peer);
            };
            _Service.Join(peer, peer.Socket);
        }
    }
}