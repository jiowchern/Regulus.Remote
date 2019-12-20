using NUnit.Framework;

namespace Regulus.Remote.Client.JIT.Test
{
    public interface ITestable
    {
        void Method1();
    }

    
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AgentProivderCreateTcp()
        {
            Regulus.Remote.Client.JIT.AgentProivder.CreateTcp(Protocol.Essential.CreateFromDomain(typeof(ITestable).Assembly));
            Assert.Pass();
        }



        [Test]
        public void AgentProivderCreateRudp()
        {
            Regulus.Remote.Client.JIT.AgentProivder.CreateRudp(Protocol.Essential.CreateFromDomain(typeof(ITestable).Assembly));
            Assert.Pass();
        }

       

    }
}