
namespace Regulus.Remote.Server
{
    public static class Provider
    {
        public static Soul.IService CreateService(IEntry entry,  IProtocol protocol,Regulus.Serialization.ISerializable serializable)
        {
            return new Soul.Service(entry, protocol, serializable , new Regulus.Remote.InternalSerializer());
        }

        public static Tcp.Listener CreateTcp(Soul.IService service)
        {
            return new Tcp.Listener(service);
        }

        public static Regulus.Remote.Server.WebSocket.Listener CreateWebSocket(Soul.IService service)
        {
            return new Regulus.Remote.Server.WebSocket.Listener(service);
        }
    }
}
