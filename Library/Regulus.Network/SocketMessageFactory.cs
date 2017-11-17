

using Regulus.Network.Package;

namespace Regulus.Network
{
    public class SocketMessageFactory : Utility.Singleton<SocketMessageFactory> 
    {
        private readonly ObjectFactory<SocketMessageInternal, SocketMessage> _Factory;
        public SocketMessageFactory()
        {
            _Factory = new ObjectFactory<SocketMessageInternal, SocketMessage>(new SocketMessageProvider(Config.Default.PackageSize));
        }

        public SocketMessageFactory(int PackageSize)
        {
            _Factory = new ObjectFactory<SocketMessageInternal, SocketMessage>(new SocketMessageProvider(PackageSize));
        }

        public SocketMessage Spawn()
        {
            return _Factory.Spawn();
        }
    }

    
}