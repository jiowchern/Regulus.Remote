using System;
using System.Net;

namespace Regulus.Network.RUDP
{
    public interface ISocketRecevieable
    {        
        SocketMessage[] Received();        
    }
}