using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Tcp
{
    public class Peer : IPeer
    {
        private readonly System.Net.Sockets.Socket m_Socket;
        private Action<int, SocketError> m_ReadedHandler;
        private Action<int, SocketError> m_SendDoneHandler;


        public Peer(System.Net.Sockets.Socket Socket)
        {
            m_Socket = Socket;            
        }

        EndPoint IPeer.RemoteEndPoint => m_Socket.RemoteEndPoint;

        EndPoint IPeer.LocalEndPoint => m_Socket.LocalEndPoint;

        bool IPeer.Connected => m_Socket.Connected;

        void IPeer.Receive(byte[] ReadedByte, int Offset, int Count, Action<int, SocketError> Readed)
        {
            m_ReadedHandler = Readed;
            m_Socket.BeginReceive(ReadedByte, Offset, Count, SocketFlags.None, this.Readed, state: null);
        }

        void IPeer.Send(byte[] Buffer, int OffsetI, int BufferLength, Action<int, SocketError> WriteCompletion)
        {
            m_SendDoneHandler = WriteCompletion;
            m_Socket.BeginSend(Buffer, OffsetI, BufferLength,SocketFlags.None, SendDone, state: null);
        }

        void IPeer.Close()
        {
            if (m_Socket.Connected)
                m_Socket.Shutdown(SocketShutdown.Both);
            m_Socket.Close();
        }

        private void SendDone(IAsyncResult Ar)
        {
            SocketError error;
            var sendCount = m_Socket.EndSend(Ar , out error);
            m_SendDoneHandler(sendCount , error);
        }

        private void Readed(IAsyncResult Ar)
        {
            SocketError error;
            var readCount = m_Socket.EndReceive(Ar, out error);

            m_ReadedHandler(readCount , error);
        }

        protected System.Net.Sockets.Socket GetSocket()
        {
            return m_Socket;
        }
    }

    
}