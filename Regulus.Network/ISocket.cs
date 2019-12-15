namespace Regulus.Network
{
    public interface ISocket : ISocketRecevieable , ISocketSendable
    {
        void Close();
        void Bind(int Port);
    }
}