using System;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.RUDP
{

    public enum PEER_STATUS
    {
        CLOSE,
        CONNECTING,
        TRANSMISSION,
        DISCONNECT
    }
    public interface IPeer
    {
        EndPoint EndPoint { get;  }
        void Receive(byte[] buffer, int offset, int count, Action<int, SocketError> read_completion);
        void Send(byte[] buffer, int offset, int count, Action<int, SocketError> write_completion);
        PEER_STATUS Status { get; }

        void Disconnect();
    }


}