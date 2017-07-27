using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Regulus.Network.RUDP
{
    class SocketSender : ISendable
    {
        private readonly Socket _Socket;
        
        
        
        public SocketSender(Socket socket)
        {
            _Socket = socket;

        
        }
        
        public void Transport(SocketMessage message)
        {
            _Transport(message);
        }

        private event Action<SocketMessage> _DoneEvent;

        event Action<SocketMessage> ISendable.DoneEvent
        {
            add { this._DoneEvent += value; }
            remove { this._DoneEvent -= value; }
        }





        private void _Transport(SocketMessage message)
        {
            _Socket.BeginSendTo(message.Package, 0, message.GetPackageSize(), SocketFlags.None, message.RemoteEndPoint, _Done,
                message);
        }

        private void _Done(IAsyncResult ar)
        {
            
            _Socket.EndSendTo(ar);

            var message = ar.AsyncState as SocketMessage;
            _DoneEvent(message);            
        }        
    }
}