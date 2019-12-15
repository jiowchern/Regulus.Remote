

using Regulus.Network.Package;

namespace Regulus.Network
{
    public class SocketMessageProvider : IObjectProvider<SocketMessageInternal>
    {
        private readonly int m_PackageSize;

        public SocketMessageProvider(int PackageSize)
        {
            m_PackageSize = PackageSize;
        }
        SocketMessageInternal IObjectProvider<SocketMessageInternal>.Spawn()
        {
            return new SocketMessageInternal(m_PackageSize);
        }
    }
}