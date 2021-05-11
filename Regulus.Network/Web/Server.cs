using Regulus.Utility;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Regulus.Network.Web
{
    public class Listener 
    {
        readonly System.Net.HttpListener _HttpListener;
        private readonly System.Threading.CancellationTokenSource _CancelGetContext;

        public Listener()
        {
            _HttpListener = new System.Net.HttpListener();
            _CancelGetContext = new System.Threading.CancellationTokenSource();


        }
        event Action<Peer> _AcceptEvent;
        public event Action<Peer> AcceptEvent
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

        public void Bind(string address)
        {
            _HttpListener.Prefixes.Add(address);
            _HttpListener.Start();
            _ = _HttpListener.GetContextAsync().ContinueWith(_Listen, _CancelGetContext.Token);
        }

        private void _Listen(Task<HttpListenerContext> task)
        {
            _ = _HttpListener.GetContextAsync().ContinueWith(_Listen, _CancelGetContext.Token);
            if (task.IsCanceled)
                return;
            if (!task.IsCompleted)
            {
                return;   
            }

            HttpListenerContext context = task.Result;
            if (context.Request.IsWebSocketRequest)
            {
                _Accept(context);
            }
        }

        private async void _Accept(HttpListenerContext context)
        {
            
            System.Net.WebSockets.HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);

            var webSocket = webSocketContext.WebSocket;


            var status = await Task<System.Net.WebSockets.WebSocketState>.Factory.StartNew(() =>
            {
                while (webSocket.State == System.Net.WebSockets.WebSocketState.Connecting)
                {                    
                    System.Threading.Thread.Sleep(1);
                }
                return webSocket.State;
            });
            if(status == System.Net.WebSockets.WebSocketState.Open)
                _AcceptEvent(new Peer(webSocket));
        }

        public void Close()
        {
            _CancelGetContext.Cancel();
            _HttpListener.Close();

        }
    }
}
