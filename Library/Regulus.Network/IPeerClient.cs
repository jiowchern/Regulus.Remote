namespace Regulus.Network
{
    public interface IPeerClient
    {
        void Launch();
        void Shutdown();

        IPeerConnectable Spawn();
    }
}