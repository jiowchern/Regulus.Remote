using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Regulus.Network.Web
{
    public class Peer : IStreamable
    {
        private readonly WebSocket _Socket;
        



        public Peer(WebSocket socket)
        {
            _Socket = socket;            
        }        

        System.Threading.Tasks.Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            System.Threading.CancellationToken cancellationToken = default;
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, count);
            return _Socket.ReceiveAsync(segment, cancellationToken).ContinueWith<int>((t) =>
            {
                try
                {
                    WebSocketReceiveResult r = t.Result;
                    return r.Count;
                }
                catch (Exception e)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(_Socket.CloseStatus.ToString());
                    
                    
                }
                return 0;
            });
        }

        System.Threading.Tasks.Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            ArraySegment<byte> a = new ArraySegment<byte>(buffer, offset, count);

            System.Threading.CancellationToken cancellationToken = default;

            return _Socket.SendAsync(a, WebSocketMessageType.Binary, true, cancellationToken).ContinueWith<int>((t) =>
            {
                return count - offset;
            });


        }

        public bool IsValid()
        {
            return _Socket.State == WebSocketState.Open;
        }
    }
}
