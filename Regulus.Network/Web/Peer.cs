using System;
using System.Net;
using System.Net.WebSockets;

namespace Regulus.Network.Web
{
    public class Peer : IStreamable
    {
        private readonly WebSocket _Socket;
        private readonly IPEndPoint localEndPoint;
        private readonly IPEndPoint remoteEndPoint;

        

        public Peer(WebSocket socket, IPEndPoint localEndPoint, IPEndPoint remoteEndPoint) 
        {
            _Socket = socket;
            this.localEndPoint = localEndPoint;
            this.remoteEndPoint = remoteEndPoint;
        }

        

        

        

        System.Threading.Tasks.Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            System.Threading.CancellationToken cancellationToken = default;
            var segment = new ArraySegment<byte>(buffer , offset , count);
            return _Socket.ReceiveAsync(segment, cancellationToken).ContinueWith<int>((t)=> {
                var r= t.Result;
               return r.Count;
            });
        }

        System.Threading.Tasks.Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            var a = new ArraySegment<byte>(buffer, offset, count);
            
            System.Threading.CancellationToken cancellationToken = default;
            
            return _Socket.SendAsync(a , WebSocketMessageType.Binary, true, cancellationToken).ContinueWith<int>((t)=> {
                return count- offset;
            });

            
        }
    }
}
