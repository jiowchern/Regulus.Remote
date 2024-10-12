using System;
using System.Net.Sockets;

namespace Regulus.Network.Tcp
{
    using Regulus.Remote;
    public class Peer : IStreamable 
    {
        public readonly System.Net.Sockets.Socket Socket;
        readonly SockerTransactor _Send;
        readonly SockerTransactor _Receive;
        public Peer(System.Net.Sockets.Socket socket )
        {
                        
            Socket = socket;            
            _Receive = new SockerTransactor(Socket.BeginReceive , _EndReceive );
            _Receive.SocketErrorEvent += (e) => { ReceiveEvent(0); };
            _Send = new SockerTransactor(Socket.BeginSend, _EndSend);
            _Send.SocketErrorEvent += (e) => { SendEvent(0); };

            _SocketErrorEvent += (e) => { BreakEvent(); };
            ReceiveEvent += (size) => { if(size == 0) BreakEvent(); };
            SendEvent += (size) => { if (size == 0) BreakEvent(); };
        }

        public event System.Action BreakEvent ;
        public event System.Action<int> ReceiveEvent;
        public event System.Action<int> SendEvent;
        event Action<SocketError> _SocketErrorEvent;
        public event Action<SocketError> SocketErrorEvent
        {
            add
            {
                _SocketErrorEvent += value;
                _Receive.SocketErrorEvent += value;
                _Send.SocketErrorEvent += value;
            }

            remove
            {
                _SocketErrorEvent -= value;
                _Receive.SocketErrorEvent -= value;
                _Send.SocketErrorEvent -= value;
            }
        }

        IWaitableValue<int> IStreamable.Receive(byte[] readed_byte, int offset, int count)
        {
            return _Receive.Transact(readed_byte, offset, count);                  
        }        
        private int _EndReceive(IAsyncResult arg)
        {

            SocketError error;
            
            int size = Socket.EndReceive(arg, out error);
            ReceiveEvent(size);
            if (error == SocketError.Success)
                return size;
            
            _SocketErrorEvent(error);
            return size;
        }
        IWaitableValue<int> IStreamable.Send(byte[] buffer, int offset, int buffer_length)
        {            
            return _Send.Transact(buffer, offset, buffer_length);
            
        }

        private int _EndSend(IAsyncResult arg)
        {
            SocketError error;

            int size = Socket.EndSend(arg, out error);
            SendEvent(size);
            if (error == SocketError.Success)
                return size;

            _SocketErrorEvent(error);
            return size;
        }
        protected System.Net.Sockets.Socket GetSocket()
        {
            return Socket;
        }

        
    }


}