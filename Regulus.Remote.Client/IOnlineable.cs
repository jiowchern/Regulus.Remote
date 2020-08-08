using System.Net.Sockets;

namespace Regulus.Remote.Client.Tcp
{
    public interface IOnlineable
    {
        event System.Action<SocketError> ErrorEvent;
        void Disconnect();
    }
}