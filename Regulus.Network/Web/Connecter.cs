using System;
using System.Net;
using System.Net.WebSockets;

namespace Regulus.Network.Web
{
    public class Connecter :  Peer , IConnectable
    {
        readonly ClientWebSocket _Socket;

        public Connecter(ClientWebSocket socket) : base(socket , new IPEndPoint(IPAddress.Any,0), new IPEndPoint(IPAddress.Any, 0))
        {
            _Socket = socket;
        }

        void IConnectable.Connect(EndPoint endpoint, Action<bool> Result)
        {
            var ip = endpoint as IPEndPoint;
            var t = _Socket.ConnectAsync(new Uri($"ws://{ip.Address.ToString()}:{ip.Port}") , new System.Threading.CancellationToken());
            t.Wait();
            if(_Socket.State == WebSocketState.Open)
            {
                Result(true);
            }
            else
            {
                Result(false);
            }

        }


    }
}
