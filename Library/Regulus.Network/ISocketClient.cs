namespace Regulus.Network
{
    public interface ISocketClient
    {
        void Launch();
        void Shutdown();

        ISocketConnectable Spawn();
    }
}