

namespace Regulus.Framework.Client.JIT.Tests
{

    public class Test
    {

    }
    
    public class AgentProivderTests
    {
        [NUnit.Framework.Test()]
        public void CreateRudpTest()
        {

            Regulus.Framework.Client.JIT.AgentProivder.CreateRudp(typeof(Test).Assembly);
        }

        [NUnit.Framework.Test()]
        public void CreateTcpTest()
        {

            Regulus.Framework.Client.JIT.AgentProivder.CreateTcp(typeof(Test).Assembly);
        }
    }
}