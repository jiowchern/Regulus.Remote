using System;
using System.Diagnostics.Eventing.Reader;
using System.Net;

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
        void Send(byte[] buffer);
        byte[] Receive();
        PEER_STATUS Status { get; }
    }


}