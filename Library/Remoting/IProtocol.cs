using Regulus.Serialization;

namespace Regulus.Remote
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