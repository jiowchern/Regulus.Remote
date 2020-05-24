using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Regulus.Network.Tcp
{
    public class Peer : IPeer
    {
        private readonly System.Net.Sockets.Socket _Socket;
        


        private bool _Enable;
        public Peer(System.Net.Sockets.Socket socket)
        {
            _Enable = true;
            _Socket = socket;            
        }

        EndPoint IPeer.RemoteEndPoint {get { return _Socket.RemoteEndPoint; } }

        EndPoint IPeer.LocalEndPoint { get { return _Socket.LocalEndPoint; } }

        bool IPeer.Connected
        {
            get { return _Socket.Connected && _Enable; }
        }

        void IPeer.Receive(byte[] readed_byte, int offset, int count ,Action<int> done)
        {
            
            

            try
            {
                
                _Socket.BeginReceive(readed_byte, offset, count, SocketFlags.None, this._Readed, state: done);
            }
            catch (Exception e)
            {
                _Enable = false;
            }

            
        }

        Task IPeer.Send(byte[] buffer, int offset_i, int buffer_length)
        {
            var task = new Task() { Buffer = buffer, Offset = offset_i, Count = buffer_length };
            try
            {
                _Socket.BeginSend(buffer, offset_i, buffer_length, SocketFlags.None, SendDone, state: task);
            }
            catch (Exception e)
            {
                _Enable = false;
            }
            return task;
        }

        void IPeer.Close()
        {
            if (_Socket.Connected)
                _Socket.Shutdown(SocketShutdown.Both);
            _Socket.Close();
        }

        private void SendDone(IAsyncResult ar)
        {
            

            if (!_Socket.Connected)
                return ;
            
            try
            {
                SocketError error;
                var sendCount = _Socket.EndSend(ar, out error);
                var task = (Task) ar.AsyncState;
                task.Done(sendCount);
            }
            catch (Exception e)
            {
                _Enable = false;
            }
            
        }

        private void _Readed(IAsyncResult ar)
        {
            

            if (!_Socket.Connected)
                return;

            var task = (Action<int>)ar.AsyncState;
            try
            {
                SocketError error;
                var readCount = _Socket.EndReceive(ar, out error);
                task(readCount);                
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