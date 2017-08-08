using System.Data;

namespace Regulus.Network.RUDP
{
    public class SocketPackagePool : Utility.Singleton<SocketPackagePool> , ISocketPackageSpawner
    {
        private readonly ObjectPool<SocketMessageInternal, SocketMessage> _Pool;
        public SocketPackagePool()
        {
            _Pool = new ObjectPool<SocketMessageInternal, SocketMessage>(new SocketMessageFactory(Config.Default.PackageSize));
        }

        public SocketPackagePool(int package_size)
        {
            _Pool = new ObjectPool<SocketMessageInternal, SocketMessage>(new SocketMessageFactory(package_size));
        }

        public SocketMessage Spawn()
        {
            return _Pool.Spawn();
        }
    }

    public interface ISocketPackageSpawner
    {
        SocketMessage Spawn();
    }
}