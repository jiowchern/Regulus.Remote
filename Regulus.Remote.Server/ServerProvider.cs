
namespace Regulus.Remote.Server
{
    public static class Provider
    {
        public static Soul.IService CreateService(IEntry entry,  IProtocol protocol)
        {
            return new Soul.Service(entry, protocol);
        }

        public static TcpSocket CreateTcp(Soul.IService service)
        {
            return new TcpSocket(service);
        }
    }
}
