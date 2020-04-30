using NUnit.Framework;
using Regulus.Remote;
using System.Linq;

namespace Regulus.Application.Client.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InterfaceProviderTyepsTest()
        {
            var types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IType), typeof(CType));
            var ip = new InterfaceProvider(types);

            var type = ip.Types.First();

            Assert.AreEqual(type, typeof(IType));


        }


        [Test]
        public void AgentEventRectifierSupplyTest()
        {
            var types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IType), typeof(CType));
            var ip = new InterfaceProvider(types);

            TestAgent agent = new TestAgent();
            var cType = new CType(System.Guid.NewGuid());

            object outSupplyInstance = null;

            using (var rectifier = new Remote.Client.AgentEventRectifier(ip.Types, agent))
            {

                rectifier.SupplyEvent += (type, instance) =>
                {
                    outSupplyInstance = instance;
                };

                agent.Add(typeof(IType), cType);

            }
            Assert.AreEqual(outSupplyInstance, cType);

        }

        [Test]
        public void AgentEventRectifierUnsupplyTest()
        {
            var types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IType), typeof(CType));
            var ip = new InterfaceProvider(types);


            TestAgent agent = new TestAgent();
            var cType = new CType(System.Guid.NewGuid());

            object outInstance = null;
            System.Type outType = null;
            using (var rectifier = new Remote.Client.AgentEventRectifier(ip.Types, agent))
            {
                rectifier.SupplyEvent += (type, instance) => { };
                rectifier.UnsupplyEvent += (type, instance) =>
                {
                    outInstance = instance;
                    outType = type;
                };

                agent.Add(typeof(IType), cType);

                agent.Remove(typeof(IType), cType);

            }
            Assert.AreEqual(outInstance, cType);
            Assert.AreEqual(typeof(IType), outType);

        }
        [Test]
        public void MethodStringInvokerTest1()
        {
            var test = new CType(System.Guid.NewGuid());
            var method = typeof(IType).GetMethod(nameof(IType.TestMethod1));
            var invoker = new Remote.Client.MethodStringInvoker(test, method);
            invoker.Invoke("1", "2", "3");

            Assert.AreEqual(true, test.TestMethod1Invoked);


        }

        [Test]
        public void AgentCommandTest1()
        {
            var test = new CType(System.Guid.NewGuid());
            var method = typeof(IType).GetMethod(nameof(IType.TestMethod1));
            var invoker = new Remote.Client.MethodStringInvoker(test, method);
            var agentCommand = new Regulus.Remote.Client.AgentCommand(new Remote.Client.AgentCommandVersionProvider(), typeof(IType), invoker);
            Assert.AreEqual("IType-0.TestMethod1[a1,a2,a3]", agentCommand.Name);

        }
        [Test]
        public void AgentCommandRegisterTest1()
        {

            var command = new Utility.Command();
            int regCount = 0;
            command.RegisterEvent += (cmd, ret, args) => {
                regCount++;
            };
            int unregCount = 0;
            command.UnregisterEvent += (cmd) => {
                unregCount++;
            };
            var agentCommandRegister = new Remote.Client.AgentCommandRegister(command);

            var test = new CType(System.Guid.NewGuid());

            agentCommandRegister.Regist(typeof(IType), test);
            agentCommandRegister.Unregist(test);

            Assert.AreEqual(2 , regCount);
            Assert.AreEqual(2, unregCount);
        }
        
    }
}