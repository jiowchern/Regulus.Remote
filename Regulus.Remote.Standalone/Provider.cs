namespace Regulus.Remote.Standalone
{
    public static class Provider
    {
        public static Service CreateService(IBinderProvider entry, IProtocol protocol)
        {
            return CreateService(entry, protocol, new Regulus.Remote.Serializer(protocol.SerializeTypes));
        }
        public static Service CreateService(IBinderProvider entry , IProtocol protocol, ISerializable serializable )
        {
            return new Standalone.Service(entry, protocol, serializable , new Regulus.Remote.InternalSerializer());
        }

       
    }
}
