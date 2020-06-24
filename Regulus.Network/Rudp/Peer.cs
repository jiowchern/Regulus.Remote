using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Rudp
{
    internal class Peer : IPeer
    {
        private readonly Regulus.Network.Socket _RudpSocket;

        public Peer(Regulus.Network.Socket rudp_socket)
        {
            _RudpSocket = rudp_socket;
            
        }

        EndPoint IPeer.RemoteEndPoint { get { return _RudpSocket.EndPoint; } }

        EndPoint IPeer.LocalEndPoint {get { return _RudpSocket.EndPoint; } }

        bool IPeer.Connected { get { return _RudpSocket.Status == PeerStatus.Transmission; } }

        System.Threading.Tasks.Task<int> IPeer.Receive(byte[] buffer, int offset, int count)
        {
            return _RudpSocket.Receive(buffer, offset, count);
            
        }
        System.Threading.Tasks.Task<int> IPeer.Send(byte[] buffer, int offset, int length)
        {
            return _RudpSocket.Send(buffer , offset , length );
        }

        void IPeer.Close()
        {
            _RudpSocket.Disconnect();
        }
    }
}