namespace Regulus.Remote.Standalone
{
    public class Provider
    {
        public static Standalone.IService CreateService(IProtocol protocol, ISerializable serializable, IBinderProvider entry)
        {
            return new Standalone.Service(entry, protocol, serializable , new Regulus.Remote.InternalSerializer());
        }
    }
}
