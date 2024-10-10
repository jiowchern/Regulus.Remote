namespace Regulus.Remote.Standalone
{
    public static class Provider
    {
        public static Service CreateService(IEntry entry, IProtocol protocol)
        {

            return CreateService(entry, protocol, new Regulus.Remote.Serializer(protocol.SerializeTypes),Regulus.Memorys.PoolProvider.Shared );
        }
        public static Service CreateService(IEntry entry , IProtocol protocol, ISerializable serializable ,Regulus.Memorys.IPool pool)
        {
            return new Standalone.Service(entry, protocol, serializable , new Regulus.Remote.InternalSerializer(), pool);
        }

       
    }
}
