using Regulus.Network.Package;

namespace Regulus.Network
{
    public interface ISocketRecevieable
    {        
        SocketMessage[] Received();        
    }
}