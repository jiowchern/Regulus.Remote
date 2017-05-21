using Regulus.Serialization;

namespace Regulus.Remoting
{
    public interface IProtocol
    {
        EventProvider GetEventProvider();
        InterfaceProvider GetInterfaceProvider();

        ISerializer GetSerialize();

        MemberMap GetMemberMap();

        byte[] VerificationCode { get; }
    }
}