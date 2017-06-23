using System;

namespace Regulus.Network.RUDP
{
    public interface IRecevieable
    {
        event Action<SocketPackage> ReceivedEvent;
    }
}