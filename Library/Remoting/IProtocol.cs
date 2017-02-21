namespace Regulus.Remoting
{
    public interface IProtocol
    {
        EventProvider GetEventProvider();
        GPIProvider GetGPIProvider();
    }
}