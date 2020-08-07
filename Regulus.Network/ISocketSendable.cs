using Regulus.Network.Package;

namespace Regulus.Network
{
    public interface ISocketSendable
    {
        void Transport(SocketMessage Message);

    }
}