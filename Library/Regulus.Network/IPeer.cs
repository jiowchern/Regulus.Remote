using System;
using System.Diagnostics.Eventing.Reader;
using System.Net;

namespace Regulus.Network.RUDP
{
    public interface IPeer
    {
        EndPoint EndPoint { get;  }
        void Send(byte[] buffer);

        event Action<byte[]> ReceivedEvent;

        event Action TimeoutEvent;
    }
}