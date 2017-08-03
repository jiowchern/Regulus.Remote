using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Regulus.Extension;

namespace Regulus.Network.RUDP
{
    class SocketSender 
    {
        private readonly Socket _Socket;

        private readonly Queue<SocketMessage> _Messages;
        
        public SocketSender(Socket socket)
        {
            _Messages = new Queue<SocketMessage>();
            _Socket = socket;        
        }
        
        public void Transport(SocketMessage message)
        {
            lock (_Messages)
            {
                _Messages.Enqueue(message);
                if (_Messages.Count == 1)
                {
                    _Transport(_Messages.Dequeue());
                }
            }
            
            
        }

        private event Action<SocketMessage> _DoneEvent;

        public event Action<SocketMessage> DoneEvent
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
            SocketError error = SocketError.Success;
            try
            {
                _Socket.EndSendTo(ar);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                error = e.SocketErrorCode;
            }
            finally
            {
                var message = ar.AsyncState as SocketMessage;                
                message.SetError(error);
                _DoneEvent(message);


                lock (_Messages)
                {
                    if(_Messages.Count >0)
                        _Transport(_Messages.Dequeue());
                }
            }
            

            
        }        
    }
}