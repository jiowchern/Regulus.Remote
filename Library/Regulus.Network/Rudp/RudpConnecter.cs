using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Network.RUDP;

namespace Regulus.Network
{
    public class RudpConnecter : ISocketConnectable
    {
        private readonly Agent _Agent;
        private IPeer _Peer;

        public RudpConnecter(Regulus.Network.RUDP.Agent agent)
        {
            _Agent = agent;
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
            _Peer.Receive(readed_byte , offset , count , readed);
        }

        void ISocket.Send(byte[] buffer, int offset_i, int buffer_length, Action<int, SocketError> write_completion)
        {
            _Peer.Send(buffer , offset_i , buffer_length , write_completion);
        }

        void ISocket.Close()
        {
            _Peer.Disconnect();
        }

        void ISocketConnectable.Connect(EndPoint endpoint, Action<bool> result)
        {
            _Peer = _Agent.Connect(endpoint,result);
            
        }
    }
}