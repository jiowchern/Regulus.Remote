using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Rudp
{
    
    public class Connector : IConnectable
    {
        private readonly Agent m_Agent;
        private Regulus.Network.Socket _RudpSocket;

        public Connector(Agent Agent)
        {
            m_Agent = Agent;
        }
        EndPoint IPeer.RemoteEndPoint {get { return _RudpSocket.EndPoint; } }

        EndPoint IPeer.LocalEndPoint {get { return _RudpSocket.EndPoint; } }

        bool IPeer.Connected {get{return _RudpSocket.Status == PeerStatus.Transmission; } }

        System.Threading.Tasks.Task<int> IPeer.Receive(byte[] ReadedByte, int Offset, int Count)
        {
            return _RudpSocket.Receive(ReadedByte, Offset, Count);
        }

        System.Threading.Tasks.Task<int> IPeer.Send(byte[] Buffer, int OffsetI, int BufferLength)
        {
            return _RudpSocket.Send(Buffer, OffsetI, BufferLength);
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