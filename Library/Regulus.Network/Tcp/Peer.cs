using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Regulus.Network.Tcp
{
    public class Peer : IPeer
    {
        private readonly System.Net.Sockets.Socket _Socket;
        private Action<int, SocketError> _ReadedHandler;
        private Action<int, SocketError> _SendDoneHandler;

        private int _Length;
        private bool _Enable;
        public Peer(System.Net.Sockets.Socket socket)
        {
            _Enable = true;
            _Socket = socket;            
        }

        EndPoint IPeer.RemoteEndPoint => _Socket.RemoteEndPoint;

        EndPoint IPeer.LocalEndPoint => _Socket.LocalEndPoint;

        bool IPeer.Connected => _Socket.Connected && _Enable;

        void IPeer.Receive(byte[] readed_byte, int offset, int count, Action<int, SocketError> Readed)
        {
            
            _ReadedHandler = Readed;

            try
            {
                _Socket.BeginReceive(readed_byte, offset, count, SocketFlags.None, this.Readed, state: null);
            }
            catch (Exception e)
            {
                _Enable = false;
            }
        }

        void IPeer.Send(byte[] buffer, int offset_i, int buffer_length, Action<int, SocketError> write_completion)
        {
            _Length = buffer_length;
            _SendDoneHandler = write_completion;
            try
            {
                _Socket.BeginSend(buffer, offset_i, buffer_length, SocketFlags.None, SendDone, state: null);
            }
            catch (Exception e)
            {
                _Enable = false;
            }
            
        }

        void IPeer.Close()
        {
            if (_Socket.Connected)
                _Socket.Shutdown(SocketShutdown.Both);
            _Socket.Close();
        }

        private void SendDone(IAsyncResult ar)
        {
            var handler = _SendDoneHandler;
            _SendDoneHandler = null;

            if (!_Socket.Connected)
                return ;
            
            try
            {
                SocketError error;
                var sendCount = _Socket.EndSend(ar, out error);

                if (handler != null)
                    handler(sendCount, error);
            }
            catch (Exception e)
            {
                _Enable = false;
            }
            
        }

        private void Readed(IAsyncResult ar)
        {
            var handler = _ReadedHandler;
            _ReadedHandler = null;

            if (!_Socket.Connected)
                return;
            

            try
            {
                SocketError error;
                var readCount = _Socket.EndReceive(ar, out error);

                handler(readCount, error);
            }
            catch (Exception e)
            {
                _Enable = false;
            }
            
            
        }

        protected System.Net.Sockets.Socket GetSocket()
        {
            return _Socket;
        }
    }

    
}