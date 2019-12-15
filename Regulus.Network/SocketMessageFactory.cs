

using Regulus.Network.Package;

namespace Regulus.Network
{
    public class SocketMessageFactory : Utility.Singleton<SocketMessageFactory> 
    {
        readonly int _PackageSize;
        public SocketMessageFactory()
        {
            _PackageSize = Config.Default.PackageSize;
        }

        public SocketMessageFactory(int package_size)
        {
            _PackageSize = package_size;
                }

        public SocketMessage Spawn()
        {
            return new SocketMessage(_PackageSize);
        }
    }

    
}