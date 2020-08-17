using Regulus.Serialization;

namespace Regulus.Remote
{
    public interface IProtocol
    {
        System.Reflection.Assembly Base { get; }
        EventProvider GetEventProvider();
        InterfaceProvider GetInterfaceProvider();

        ISerializer GetSerialize();

        MemberMap GetMemberMap();

        byte[] VerificationCode { get; }
    }
}