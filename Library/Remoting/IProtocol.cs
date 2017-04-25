

namespace Regulus.Remoting
{
    public interface IProtocol
    {
        EventProvider GetEventProvider();
        GPIProvider GetGPIProvider();

        ISerialize GetSerialize();
    }

    public interface ISerialize
    {
    }
}