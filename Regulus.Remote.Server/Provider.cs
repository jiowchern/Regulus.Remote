namespace Regulus.Remote.Server
{


    public static class Provider
    {

        
        public static Soul.IService CreateService(IEntry entry, IProtocol protocol, Soul.IListenable listenable)
        {
            var pool = Regulus.Memorys.PoolProvider.Shared;
            return new Soul.AsyncService(new Soul.SyncService(entry, new Soul.UserProvider(protocol, new Regulus.Remote.Serializer(protocol.SerializeTypes), listenable, new Regulus.Remote.InternalSerializer(), pool)));            
        }

        public static Soul.IService CreateService(IEntry entry,  IProtocol protocol, ISerializable serializable ,Soul.IListenable listenable)
        {
            var pool = Regulus.Memorys.PoolProvider.Shared;

            return new Soul.AsyncService(new Soul.SyncService(entry, new Soul.UserProvider(protocol, serializable, listenable, new Regulus.Remote.InternalSerializer(), pool)));            
        }

        public static TcpListenSet CreateTcpService(IEntry entry, IProtocol protocol)
        {
            return CreateTcpService(entry , protocol,new Regulus.Remote.Serializer(protocol.SerializeTypes));
        }
        public static TcpListenSet CreateTcpService( IEntry entry, IProtocol protocol, ISerializable serializable)
        {
            var listener = new Regulus.Remote.Server.Tcp.Listener();
            var service = CreateService(entry, protocol, serializable, listener);
            return new TcpListenSet(listener , service);
        }
        public static WebListenSet CreateWebService(IEntry entry, IProtocol protocol)
        {
            return CreateWebService(entry, protocol, new Regulus.Remote.Serializer(protocol.SerializeTypes));
        }
        public static WebListenSet CreateWebService(IEntry entry, IProtocol protocol, ISerializable serializable)
        {
            var listener = new Regulus.Remote.Server.Web.Listener();
            var service = CreateService(entry, protocol, serializable, listener);
            return new WebListenSet(listener, service);
        }

        
    }
}
