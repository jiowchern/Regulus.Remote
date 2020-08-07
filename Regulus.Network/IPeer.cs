using System.Net.Sockets;

namespace Regulus.Network
{
    public interface IPeer : IStreamable
    {
        event System.Action<SocketError> SocketErrorEvent;
    }
}
