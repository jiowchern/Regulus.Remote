using Regulus.Serialization;
using System;
using System.Reflection;

namespace Regulus.Remote
{
    public class NewProtocol : Remote.IProtocol
    {
        Assembly IProtocol.Base => throw new NotImplementedException();

        byte[] IProtocol.VerificationCode => throw new NotImplementedException();

        EventProvider IProtocol.GetEventProvider()
        {
            throw new NotImplementedException();
        }

        InterfaceProvider IProtocol.GetInterfaceProvider()
        {
            throw new NotImplementedException();
        }

        MemberMap IProtocol.GetMemberMap()
        {
            throw new NotImplementedException();
        }

        ISerializer IProtocol.GetSerialize()
        {
            throw new NotImplementedException();
        }
    }
}
