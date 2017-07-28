namespace Regulus.Network.RUDP
{
    public interface ISocket : ISocketRecevieable , ISocketSendable
    {
        void Close();
        void Bind(int port);
    }
}