using System;

namespace Regulus.Network.RUDP
{
    public interface ISocketSendable
    {
        void Transport(SocketMessage message);

        event Action<SocketMessage> DoneEvent;
    }
}