using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Rudp
{
    public class Connecter : IConnectable
    {
        private readonly Agent m_Agent;
        private Regulus.Network.Socket _RudpSocket;

        public Connecter(Agent Agent)
        {
            m_Agent = Agent;
        }
        EndPoint IPeer.RemoteEndPoint {get { return _RudpSocket.EndPoint; } }

        EndPoint IPeer.LocalEndPoint {get { return _RudpSocket.EndPoint; } }

        bool IPeer.Connected {get{return _RudpSocket.Status == PeerStatus.Transmission; } }

        void IPeer.Receive(byte[] ReadedByte, int Offset, int Count, Action<int, SocketError> Readed)
        {
            _RudpSocket.Receive(ReadedByte, Offset, Count, Readed);
        }

        void IPeer.Send(byte[] Buffer, int OffsetI, int BufferLength, Action<int, SocketError> WriteCompletion)
        {
            _RudpSocket.Send(Buffer, OffsetI, BufferLength, WriteCompletion);
        }

        void IPeer.Close()
        {
            _RudpSocket.Disconnect();
        }

        void IConnectable.Connect(EndPoint Endpoint, Action<bool> Result)
        {
            _RudpSocket = m_Agent.Connect(Endpoint, Result);

        }
    }
}