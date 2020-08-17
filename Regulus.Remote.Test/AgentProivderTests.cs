
using Regulus.Remote;
using Regulus.Serialization;
using System.Linq;
using System.Reflection;

namespace Regulus.Utility.Client.JIT.Tests
{

    public class Test
    {

    }

    public class TestProtocol : Regulus.Remote.IProtocol
    {
        byte[] IProtocol.VerificationCode => throw new System.NotImplementedException();

        Assembly IProtocol.Base => throw new System.NotImplementedException();

        EventProvider IProtocol.GetEventProvider()
        {
            throw new System.NotImplementedException();
        }

        InterfaceProvider IProtocol.GetInterfaceProvider()
        {
            throw new System.NotImplementedException();
        }

        MemberMap IProtocol.GetMemberMap()
        {
            throw new System.NotImplementedException();
        }

        ISerializer IProtocol.GetSerialize()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AgentProivderTests
    {
        [NUnit.Framework.Test()]
        public void CreateProtocol()
        {
            System.Type type = typeof(TestProtocol);

            IProtocol protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(type.Assembly);
            NUnit.Framework.Assert.AreNotEqual(protocol, null);
        }

        [NUnit.Framework.Test()]
        public void FindProtocols()
        {

            System.Type[] protocols = Regulus.Remote.Protocol.ProtocolProvider.GetProtocols().ToArray();
            NUnit.Framework.Assert.AreEqual(typeof(TestProtocol), protocols[0]);
        }

        /* todo 解除註解
         * [NUnit.Framework.Test()]
        public void CreateRudpTest()
        {
            Regulus.Remote.Client.JIT.AgentProivder.CreateRudp(typeof(Test));            
        }

        [NUnit.Framework.Test()]
        public void CreateTcpTest()
        {            
            Regulus.Remote.Client.JIT.AgentProivder.CreateTcp(typeof(Test));
        }*/
    }
}