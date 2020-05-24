using System;
using System.Net;
using System.Net.WebSockets;

namespace Regulus.Network.Web
{
    public class Peer : IPeer
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

        EndPoint IPeer.RemoteEndPoint => remoteEndPoint;

        EndPoint IPeer.LocalEndPoint => localEndPoint;

        bool IPeer.Connected => _Socket.State == WebSocketState.Open;

        void IPeer.Close()
        {
            System.Threading.CancellationToken cancellationToken = default;
            _Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "close from server", cancellationToken).Wait();
            _Socket.Dispose();
        }

        void IPeer.Receive(byte[] buffer, int offset, int count, Action<int> done)
        {
            System.Threading.CancellationToken cancellationToken = default;
            var segment = new ArraySegment<byte>(buffer , offset , count);
            _Socket.ReceiveAsync(segment, cancellationToken).ContinueWith((t)=> {
                done(segment.Count - segment.Offset);
            });
        }

        Task IPeer.Send(byte[] buffer, int offset, int count)
        {
            var a = new ArraySegment<byte>(buffer, offset, count);
            var result = new Task() { Buffer = a.Array, Offset = a.Offset, Count = a.Count };
            System.Threading.CancellationToken cancellationToken = default;
            
            _Socket.SendAsync(a , WebSocketMessageType.Binary, false, cancellationToken).ContinueWith(t=> {
                result.Done(a.Count - a.Offset);
            });

            return result;
        }
    }
}
