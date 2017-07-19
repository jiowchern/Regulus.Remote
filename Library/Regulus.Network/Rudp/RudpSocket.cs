using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Network.RUDP;

namespace Regulus.Network
{
    internal class RudpSocket : ISocket
    {
        private readonly IPeer _Peer;

        public RudpSocket(IPeer peer)
        {
            _Peer = peer;
            
        }

        EndPoint ISocket.RemoteEndPoint
        {
            get { return _Peer.EndPoint; }
        }

        EndPoint ISocket.LocalEndPoint
        {
            get { return _Peer.EndPoint; }
        }

        bool ISocket.Connected
        {
            get { return _Peer.Status == PEER_STATUS.TRANSMISSION; }
        }

        void ISocket.Receive(byte[] readed_byte, int offset, int count, Action<int, SocketError> readed)
        {
            _Peer.Receive(readed_byte, offset, count, readed);
        }
        void ISocket.Send(byte[] buffer, int offset, int length, Action<int, SocketError> write_completion)
        {
            _Peer.Send(buffer , offset , length , write_completion);
        }

        void ISocket.Close()
        {
            _Peer.Disconnect();
        }
    }
}