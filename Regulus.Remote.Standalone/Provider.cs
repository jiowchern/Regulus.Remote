namespace Regulus.Remote.Standalone
{
    public class Provider
    {
        public static Standalone.IService CreateService(IProtocol protocol, IEntry entry)
        {
            return new Standalone.Service(entry, protocol);
        }
    }
}
