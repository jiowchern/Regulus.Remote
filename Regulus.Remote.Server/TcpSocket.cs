using Regulus.Network;
using Regulus.Remote.Soul;
using System;

namespace Regulus.Remote.Server.Tcp
{
    public class Listener
    {
        private readonly IService _Service;
        readonly Regulus.Network.Tcp.Listener _Listener;
        readonly Regulus.Network.IListenable _Listenable;
        public Listener(IService service)
        {
            this._Service = service;
            
            _Listener = new Network.Tcp.Listener();
            _Listenable = _Listener;
            _Listenable.AcceptEvent += _Join;

        }
        public void Bind(int port)
        {
            _Listenable.Bind(port);
        }

        public void Close() {
            _Listenable.Close();
        }

        private void _Join(IPeer peer)
        {
            peer.SocketErrorEvent += (e) => {
                _Service.Leave(peer);
            };
            _Service.Join(peer);
        }
    }
}