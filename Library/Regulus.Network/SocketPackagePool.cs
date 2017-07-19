namespace Regulus.Network.RUDP
{
    public class SocketPackagePool : Utility.Singleton<SocketPackagePool> , ISocketPackageSpawner
    {
        private readonly ObjectPool<SocketMessageInternal, SocketMessage> _Pool;
        public SocketPackagePool()
        {
            _Pool = new ObjectPool<SocketMessageInternal, SocketMessage>(new SocketPackageFactory());
        }

        SocketMessage ISocketPackageSpawner.Spawn()
        {
            return _Pool.Spawn();
        }
    }

    public interface ISocketPackageSpawner
    {
        SocketMessage Spawn();
    }
}