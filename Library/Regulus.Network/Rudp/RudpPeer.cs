using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Network.RUDP;

namespace Regulus.Network
{
    internal class RudpPeer : IPeer
    {
        private readonly IRudpPeer _RudpPeer;

        public RudpPeer(IRudpPeer rudpPeer)
        {
            _RudpPeer = rudpPeer;
            
        }

        EndPoint IPeer.RemoteEndPoint
        {
            get { return _RudpPeer.EndPoint; }
        }

        EndPoint IPeer.LocalEndPoint
        {
            get { return _RudpPeer.EndPoint; }
        }

        bool IPeer.Connected
        {
            get { return _RudpPeer.Status == PEER_STATUS.TRANSMISSION; }
        }

        void IPeer.Receive(byte[] readed_byte, int offset, int count, Action<int, SocketError> readed)
        {
            _RudpPeer.Receive(readed_byte, offset, count, readed);
        }
        void IPeer.Send(byte[] buffer, int offset, int length, Action<int, SocketError> write_completion)
        {
            _RudpPeer.Send(buffer , offset , length , write_completion);
        }

        void IPeer.Close()
        {
            _RudpPeer.Disconnect();
        }
    }
}