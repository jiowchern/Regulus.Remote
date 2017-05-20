using Regulus.Serialization;

namespace Regulus.Remoting
{
    public interface IProtocol
    {
        EventProvider GetEventProvider();
        GPIProvider GetGPIProvider();

        ISerializer GetSerialize();

        byte[] VerificationCode { get; }
    }
}