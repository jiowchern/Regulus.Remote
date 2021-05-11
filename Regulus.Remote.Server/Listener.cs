using Regulus.Network.Web;
using Regulus.Remote.Soul;
namespace Regulus.Remote.Server.WebSocket
{
    public class Listener
    {
        private readonly IService _Service;
        readonly Regulus.Network.Web.Listener _Listener;

        public Listener(IService service)
        {
            this._Service = service;

            _Listener = new Network.Web.Listener();

            _Listener.AcceptEvent += _Join;

        }
        public void Bind(string  address)
        {
            _Listener.Bind(address);
        }

        public void Close()
        {
            _Listener.Close();
        }

        private void _Join(Peer peer)
        {
            peer.ErrorEvent += (status) => {
                _Service.Leave(peer);
            };
            _Service.Join(peer, null);
        }
    }
}
