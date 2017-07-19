using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Utility;

namespace Regulus.Network
{
    public class TcpSocket : ISocket
    {
        protected readonly Socket _Socket;
        private Action<int, SocketError> _ReadedHandler;
        private Action<int, SocketError> _SendDoneHandler;


        public TcpSocket(Socket socket)
        {
            _Socket = socket;            
        }

        EndPoint ISocket.RemoteEndPoint
        {
            get { return _Socket.RemoteEndPoint; }
        }

        EndPoint ISocket.LocalEndPoint
        {
            get { return _Socket.LocalEndPoint; }
        }

        bool ISocket.Connected
        {
            get { return _Socket.Connected; }
        }

        void ISocket.Receive(byte[] readed_byte, int offset, int count, Action<int, SocketError> readed)
        {
            _ReadedHandler = readed;
            _Socket.BeginReceive(readed_byte, offset, count, SocketFlags.None, _Readed, null);
        }

        void ISocket.Send(byte[] buffer, int offset_i, int buffer_length, Action<int, SocketError> write_completion)
        {
            _SendDoneHandler = write_completion;
            _Socket.BeginSend(buffer, offset_i, buffer_length,SocketFlags.None, _SendDone, null);
        }

        void ISocket.Close()
        {
            if (_Socket.Connected)
            {
                _Socket.Shutdown(SocketShutdown.Both);
            }            
            _Socket.Close();
        }

        private void _SendDone(IAsyncResult ar)
        {
            SocketError error;
            var sendCount = _Socket.EndSend(ar , out error);
            _SendDoneHandler(sendCount , error);
        }

        private void _Readed(IAsyncResult ar)
        {
            SocketError error;
            var readCount = _Socket.EndReceive(ar, out error);

            _ReadedHandler(readCount , error);
        }
    }

    
}