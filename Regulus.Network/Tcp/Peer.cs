using System;
using System.Linq;
using System.Net.Sockets;

namespace Regulus.Network.Tcp
{
    
    public class Peer : IPeer
    {
        private readonly System.Net.Sockets.Socket _Socket;
        
        public Peer(System.Net.Sockets.Socket socket)
        {
            _SocketErrorEvent += (e)=>{ };
            _Socket = socket;
        }

        event Action<SocketError> _SocketErrorEvent;
        event Action<SocketError> IPeer.SocketErrorEvent
        {
            add
            {
                _SocketErrorEvent += value;
            }

            remove
            {
                _SocketErrorEvent -= value;
            }
        }

        System.Threading.Tasks.Task<int> IStreamable.Receive(byte[] readed_byte, int offset, int count)
        {
            if (!_Socket.Connected)
            {
                _SocketErrorEvent( SocketError.SocketError);
                return System.Threading.Tasks.Task<int>.FromResult(0);
            }
                
            return System.Threading.Tasks.Task<int>.Factory.FromAsync(
                (handler, obj) => _Receive(readed_byte, offset, count, handler, obj), _EndReceive, null);

        }

        private IAsyncResult _Receive(byte[] readed_byte, int offset, int count, AsyncCallback handler, object obj)
        {

            SocketError error;
            var ar = _Socket.BeginReceive(readed_byte, offset, count, SocketFlags.None, out error, handler, obj);

            var safeList = new[] { SocketError.Success, SocketError.IOPending };
            if (!safeList.Any(s=>s==error))
                _SocketErrorEvent(error);
            return ar;
        }

        private int _EndReceive(IAsyncResult arg)
        {
            
            

            SocketError error;
            int size = _Socket.EndReceive(arg, out error);
            if(size == 0)
                _SocketErrorEvent(error);

            if (!_Socket.Connected)
            {
                _SocketErrorEvent(error);
                return size;
            }

            if (error == SocketError.Success)
                return size;
            
            _SocketErrorEvent(error);
            return size;
        }
        System.Threading.Tasks.Task<int> IStreamable.Send(byte[] buffer, int offset, int buffer_length)
        {
            if (!_Socket.Connected)
            {
                _SocketErrorEvent(SocketError.SocketError);
                return System.Threading.Tasks.Task<int>.FromResult(0);
            }
            return System.Threading.Tasks.Task<int>.Factory.FromAsync(
                (handler, obj) => _Send(buffer, offset, buffer_length, handler, obj), _EndSend, null);
        }

        private IAsyncResult _Send(byte[] buffer, int offset, int buffer_length, AsyncCallback handler, object obj)
        {
            SocketError error;
            var ar = _Socket.BeginSend(buffer, offset, buffer_length, SocketFlags.None,out error, handler, obj);
            var safeList = new[] { SocketError.Success, SocketError.IOPending };
            if (!safeList.Any(s => s == error))
                _SocketErrorEvent(error);
            return ar;
        }

        private int _EndSend(IAsyncResult arg)
        {
            SocketError error;
            int sendCount = _Socket.EndSend(arg, out error);
            if (!_Socket.Connected)
            {
                _SocketErrorEvent(error);
                return sendCount;
            }

            if (error == SocketError.Success)
            {
                
                return sendCount;
            }

            _SocketErrorEvent(error);
            return sendCount;
        }
        protected System.Net.Sockets.Socket GetSocket()
        {
            return _Socket;
        }
    }


}