using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Network.Web
{
    public class Listener : IListenable
    {
        readonly System.Net.HttpListener _HttpListener;
        private readonly System.Threading.CancellationTokenSource _CancelGetContext;

        public Listener()
        {
            _HttpListener = new System.Net.HttpListener();
            _CancelGetContext = new System.Threading.CancellationTokenSource();


        }
        event Action<IPeer> _AcceptEvent;
        event Action<IPeer> IListenable.AcceptEvent
        {
            add
            {
                _AcceptEvent += value;
            }

            remove
            {
                _AcceptEvent -= value;
            }
        }

        void IListenable.Bind(int port)
        {
            _HttpListener.Prefixes.Add($"http://127.0.0.1:{port}/");
            _HttpListener.Start();
            _ = _HttpListener.GetContextAsync().ContinueWith(_Listen,_CancelGetContext.Token );
        }

        private void _Listen(Task<HttpListenerContext> task)
        {
            _ = _HttpListener.GetContextAsync().ContinueWith(_Listen, _CancelGetContext.Token);
            if (task.IsCanceled)
                return;
            if (task.IsCompleted)
            {
                var context = task.Result;
                if (context.Request.IsWebSocketRequest)
                {
                    _Accept(context);
                }
            }            
            
        }

        private async void _Accept(HttpListenerContext context)
        {
            var webSocketContext = await context.AcceptWebSocketAsync(null);
            var webSocket = webSocketContext.WebSocket;
            TimeCounter timeCounter = new TimeCounter();
            while(webSocket.State != System.Net.WebSockets.WebSocketState.Open)
            {
                if(timeCounter.Ticks > 60)
                {                    
                    return;
                }
            }

            _AcceptEvent(new Peer(webSocket, context.Request.LocalEndPoint , context.Request.RemoteEndPoint));
        }

        void IListenable.Close()
        {
            _CancelGetContext.Cancel();
            _HttpListener.Close();
            
        }
    }
}
