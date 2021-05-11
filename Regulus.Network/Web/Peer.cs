using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Regulus.Network.Web
{
    public class Peer : IStreamable , IDisposable
    {
        private readonly WebSocket _Socket;
        readonly System.Threading.CancellationTokenSource _CancelSource;
        readonly Regulus.Remote.Value<WebSocketState> _Error;        
        public Peer(WebSocket socket)
        {
            _Socket = socket;

            _CancelSource = new System.Threading.CancellationTokenSource();
            _Error = new Remote.Value<WebSocketState>();

            
        }
        public event System.Action<WebSocketState> ErrorEvent {
            add {
                _Error.OnValue += value;
            }
            remove {
                _Error.OnValue -= value;
            }
        }
        void IDisposable.Dispose()
        {
            _CancelSource.Cancel();            
        }

        System.Threading.Tasks.Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, count);
            return _Socket.ReceiveAsync(segment, _CancelSource.Token).ContinueWith<int>((t) =>
            {
                try
                {
                    WebSocketReceiveResult r = t.Result;
                    return r.Count;
                }
                catch (Exception e)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(_Socket.CloseStatus.ToString());
                    _Error.SetValue(_Socket.State);


                }
                return 0;
            });
        }

        System.Threading.Tasks.Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            var arraySegment = new ArraySegment<byte>(buffer, offset, count);
            return _Socket.SendAsync(arraySegment, WebSocketMessageType.Binary, true, _CancelSource.Token).ContinueWith((t) =>
            {
                return count;
            });


        }
    }
}
