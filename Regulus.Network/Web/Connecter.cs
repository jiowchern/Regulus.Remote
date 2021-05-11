using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Regulus.Network.Web
{
    public class Connecter : Peer
    {
        
        readonly ClientWebSocket _Socket;

        public Connecter(ClientWebSocket socket) : base(socket)
        {
            _Socket = socket;
        }

        public System.Threading.Tasks.Task<bool> ConnectAsync(string address)
        {            
            Task connectTask = _Socket.ConnectAsync(new Uri(address), System.Threading.CancellationToken.None);
            return connectTask.ContinueWith<bool>(_ConnectResult);
        }

        public Task DisconnectAsync()
        {
            return _Socket.CloseAsync(WebSocketCloseStatus.NormalClosure,"close", System.Threading.CancellationToken.None);
        }

        private bool _ConnectResult(Task arg)
        {
            if (_Socket.State == WebSocketState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
