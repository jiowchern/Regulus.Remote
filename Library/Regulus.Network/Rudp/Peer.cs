using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Rudp
{
    internal class Peer : IPeer
    {
        private readonly Regulus.Network.Socket _RudpSocket;

        public Peer(Regulus.Network.Socket rudpSocket)
        {
            _RudpSocket = rudpSocket;
            
        }

        EndPoint IPeer.RemoteEndPoint => _RudpSocket.EndPoint;

        EndPoint IPeer.LocalEndPoint => _RudpSocket.EndPoint;

        bool IPeer.Connected => _RudpSocket.Status == PeerStatus.Transmission;

        void IPeer.Receive(byte[] ReadedByte, int Offset, int Count, Action<int, SocketError> Readed)
        {
            _RudpSocket.Receive(ReadedByte, Offset, Count, Readed);
        }
        void IPeer.Send(byte[] Buffer, int Offset, int Length, Action<int, SocketError> WriteCompletion)
        {
            _RudpSocket.Send(Buffer , Offset , Length , WriteCompletion);
        }

        void IPeer.Close()
        {
            _RudpSocket.Disconnect();
        }
    }
}