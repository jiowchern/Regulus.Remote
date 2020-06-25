
namespace Regulus.Remote.Server
{
    public static class ServiceProvider
    {
        public static Soul.Service CreateTcp(IEntry entry ,int port, IProtocol protocol)
        {
            return new Soul.Service(entry, port, protocol, new Regulus.Network.Tcp.Listener());
        }

        public static Soul.Service CreateRudp(IEntry entry, int port, IProtocol protocol)
        {
            return new Soul.Service(entry, port, protocol, new Regulus.Network.Rudp.Listener(new Regulus.Network.Rudp.UdpSocket()));
        }

        public static Soul.Service Create(IEntry entry, int port, IProtocol protocol, Regulus.Network.IListenable listenable)
        {
            return new Soul.Service(entry, port, protocol, listenable);
        }
    }
}
