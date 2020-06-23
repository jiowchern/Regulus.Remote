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

        System.Threading.Tasks.Task<int> IPeer.Receive(byte[] readed_byte, int offset, int count )
        {
            var task = new System.Threading.Tasks.Task<int>(() =>
            {
                SocketError error;
                var readCount = _Socket.Receive(readed_byte, offset, count, SocketFlags.None, out error);
                if(readCount == 0 && error == SocketError.Success)
                    return readCount;
                _Enable = false;
                return 0;
            });
            task.Start();
            return task;

         
            
        }

        SendTask IPeer.Send(byte[] buffer, int offset_i, int buffer_length)
        {
            var task = new SendTask() { Buffer = buffer, Offset = offset_i, Count = buffer_length };
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
                var task = (SendTask) ar.AsyncState;
                task.Done(sendCount);
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