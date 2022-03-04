namespace Regulus.Remote.Standalone
{
    public class Provider
    {
        public static Standalone.IService CreateService(IProtocol protocol,Regulus.Serialization.ISerializable serializable, IBinderProvider entry)
        {
            return new Standalone.Service(entry, protocol, serializable);
        }
    }
}
