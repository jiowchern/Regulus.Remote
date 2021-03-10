

using Regulus.Network.Package;

namespace Regulus.Network
{
    public class SocketMessageFactory  
    {
        public static readonly SocketMessageFactory Instance = Utility.Singleton<SocketMessageFactory>.Instance;
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