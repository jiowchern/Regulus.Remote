using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Regulus.Network.Tcp
{
    public class Peer : IStreamable
    {
        private readonly System.Net.Sockets.Socket _Socket;
        


        private bool _Enable;
        public Peer(System.Net.Sockets.Socket socket)
        {
            _Enable = true;
            _Socket = socket;            
        }

        

        

        System.Threading.Tasks.Task<int> IStreamable.Receive(byte[] readed_byte, int offset, int count )
        {
            if (!_Socket.Connected)
            {
                _Enable = false;
                return System.Threading.Tasks.Task<int>.FromResult(0);
            }
                
            return System.Threading.Tasks.Task<int>.Factory.FromAsync(
                (handler, obj) => _Socket.BeginReceive(readed_byte, offset, count, SocketFlags.None, handler, obj), _EndReceive, null);
          
        }
        private int _EndReceive(IAsyncResult arg)
        {
            if (!_Socket.Connected)
            {
                _Enable = false;
                return 0;
            }
            SocketError error;
           
                
            var size = _Socket.EndReceive(arg , out error);
            if(error == SocketError.Success)
                return size;
            _Enable = false;
            return size;
        }

       
         
            
        

        System.Threading.Tasks.Task<int> IStreamable.Send(byte[] buffer, int offset, int buffer_length)
        {
            return System.Threading.Tasks.Task<int>.Factory.FromAsync(
                (handler, obj) => _Socket.BeginSend(buffer, offset, buffer_length, SocketFlags.None, handler, obj), _EndSend, null);

        }

        private int _EndSend(IAsyncResult arg)
        {
            if (!_Socket.Connected)
            {
                _Enable = false;
                return 0;
            }

            SocketError error;
            var sendCount = _Socket.EndSend(arg , out error);
            if (error != SocketError.Success)
            {
                _Enable = false;
                return sendCount;
            }

            
            return sendCount;
        }

        

        protected System.Net.Sockets.Socket GetSocket()
        {
            return _Socket;
        }
    }

    
}