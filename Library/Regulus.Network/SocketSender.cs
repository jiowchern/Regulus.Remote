using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Regulus.Network.RUDP
{
    class SocketSender : ISendable
    {
        private readonly Socket _Socket;
        
        private readonly Queue<SocketMessage> _Packages;

        private readonly object _SendingFlag;
        private SocketMessage _Sending;
        public SocketSender(Socket socket)
        {
            _Socket = socket;
            _SendingFlag = new object();
            _Packages = new Queue<SocketMessage>();
        }
        
        public void Transport(SocketMessage message)
        {
            if (_IsSending())
            {
                _Enqueue(message);
            }
            else
            {
                _Transport(message);
            }
        }

        private bool _IsSending()
        {
            return _Sending != null;
        }

        private void _Enqueue(SocketMessage message)
        {
            lock (_Packages)
            {
                _Packages.Enqueue(message);
            }
        }

        private void _Transport(SocketMessage message)
        {
            lock (_SendingFlag)
            {
                _Sending = message;
            }
            
            _Socket.BeginSendTo(message.Package, 0, message.Package.Length, SocketFlags.None, message.EndPoint, _Done,
                null);
        }

        private void _Done(IAsyncResult ar)
        {
            
            _Socket.EndSendTo(ar);
            lock (_SendingFlag)
            {
                _Sending = null;
            }

            var pkg = _Dequeue();
            if(pkg != null)
                _Transport(pkg);
            
        }

        private SocketMessage _Dequeue()
        {
            lock (_Packages)
            {
                if(_Packages.Count > 0)
                    return _Packages.Dequeue();
            }

            return null;
        }
    }
}