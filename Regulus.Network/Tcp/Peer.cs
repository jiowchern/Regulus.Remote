using System;
using System.Linq;
using System.Net.Sockets;

namespace Regulus.Network.Tcp
{
    
    public class Peer : IStreamable , IDisposable
    {
        public readonly System.Net.Sockets.Socket Socket;

        

        public Peer(System.Net.Sockets.Socket socket)
        {
            _SocketErrorEvent += (e)=>{ };
            Socket = socket;
        }

        event Action<SocketError> _SocketErrorEvent;
        public event Action<SocketError> SocketErrorEvent
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

        IWaitableValue<int> IStreamable.Receive(byte[] readed_byte, int offset, int count)
        {
            /*if (!Socket.Connected)
            {
                _SocketErrorEvent( SocketError.SocketError);
                return (0).ToWaitableValue();
            }*/

            SocketError error;
            var ar = Socket.BeginReceive(readed_byte, offset, count, SocketFlags.None, out error, _EndReceiveEmpty, null);

            var safeList = new[] { SocketError.Success, SocketError.IOPending };
            if (!safeList.Any(s => s == error))
                _SocketErrorEvent(error);

            if (ar == null)
                return (0).ToWaitableValue();

            return System.Threading.Tasks.Task<int>.Factory.FromAsync(ar, _EndReceive).ToWaitableValue();            
        }
        
        private void _EndReceiveEmpty(IAsyncResult arg)
        {

        }
        private int _EndReceive(IAsyncResult arg)
        {
            
            

            SocketError error;
            int size = Socket.EndReceive(arg, out error);
            if(size == 0)
                _SocketErrorEvent(error);

            if (!Socket.Connected)
            {
                _SocketErrorEvent(error);
                return size;
            }

            if (error == SocketError.Success)
                return size;
            
            _SocketErrorEvent(error);
            return size;
        }
        IWaitableValue<int> IStreamable.Send(byte[] buffer, int offset, int buffer_length)
        {
            /*if (!Socket.Connected)
            {
                _SocketErrorEvent(SocketError.SocketError);
                return (0).ToWaitableValue();
            }*/

            SocketError error;
            var ar = Socket.BeginSend(buffer, offset, buffer_length, SocketFlags.None, out error, _EndReceiveEmpty, null);

            var safeList = new[] { SocketError.Success, SocketError.IOPending };
            if (!safeList.Any(s => s == error))
                _SocketErrorEvent(error);

            if (ar == null)
                return (0).ToWaitableValue();

            return System.Threading.Tasks.Task<int>.Factory.FromAsync(ar, _EndSend).ToWaitableValue();
        }

        private IAsyncResult _Send(byte[] buffer, int offset, int buffer_length, AsyncCallback handler, object obj)
        {
            SocketError error;
            var ar = Socket.BeginSend(buffer, offset, buffer_length, SocketFlags.None,out error, handler, obj);
            var safeList = new[] { SocketError.Success, SocketError.IOPending };
            if (!safeList.Any(s => s == error))
                _SocketErrorEvent(error);
            return ar;
        }

        private int _EndSend(IAsyncResult arg)
        {
            SocketError error;
            int sendCount = Socket.EndSend(arg, out error);
            if (!Socket.Connected)
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
            return Socket;
        }

        void IDisposable.Dispose()
        {
        }
    }


}