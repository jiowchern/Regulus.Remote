using Xunit;
using Regulus.Remote;
using System.Linq;

namespace Regulus.Integration.Tests
{
    public class Tests
    {


        [Fact]
        public void InterfaceProviderTyepsTest()
        {
            System.Collections.Generic.Dictionary<System.Type, System.Type> types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IType), typeof(CType));
            InterfaceProvider ip = new InterfaceProvider(types);

            System.Type type = ip.Types.First();

            Assert.Equal(type, typeof(IType));


        }


        [Fact]
        public void AgentEventRectifierSupplyTest()
        {
            System.Collections.Generic.Dictionary<System.Type, System.Type> types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IType), typeof(CType));
            InterfaceProvider ip = new InterfaceProvider(types);

            TestAgent agent = new TestAgent();
            CType cType = new CType(1);

            object outSupplyInstance = null;

            using (Remote.Client.AgentEventRectifier rectifier = new Remote.Client.AgentEventRectifier(ip.Types, agent))
            {

                rectifier.SupplyEvent += (type, instance) =>
                {
                    outSupplyInstance = instance;
                };

                agent.Add(typeof(IType), cType);

            }
            Assert.Equal(outSupplyInstance, cType);

        }

        [Fact]
        public void AgentEventRectifierUnsupplyTest()
        {
            System.Collections.Generic.Dictionary<System.Type, System.Type> types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IType), typeof(CType));
            InterfaceProvider ip = new InterfaceProvider(types);


            TestAgent agent = new TestAgent();
            CType cType = new CType(1);

            object outInstance = null;
            System.Type outType = null;
            using (Remote.Client.AgentEventRectifier rectifier = new Remote.Client.AgentEventRectifier(ip.Types, agent))
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
            Assert.Equal(outInstance, cType);
            Assert.Equal(typeof(IType), outType);

        }
        [Fact]
        public void MethodStringInvokerTest1()
        {
            CType test = new CType(1);
            System.Reflection.MethodInfo method = typeof(IType).GetMethod(nameof(IType.TestMethod1));
            Remote.Client.MethodStringInvoker invoker = new Remote.Client.MethodStringInvoker(test, method, new Remote.Client.TypeConverterSet());
            invoker.Invoke("1", "2", "3");

            Assert.Equal(true, test.TestMethod1Invoked);


        }

        [Fact]
        public void AgentCommandTest1()
        {
            CType test = new CType(1);
            System.Reflection.MethodInfo method = typeof(IType).GetMethod(nameof(IType.TestMethod1));
            Remote.Client.MethodStringInvoker invoker = new Remote.Client.MethodStringInvoker(test, method, new Remote.Client.TypeConverterSet());
            Remote.Client.AgentCommand agentCommand = new Remote.Client.AgentCommand(new Remote.Client.AgentCommandVersionProvider(), typeof(IType), invoker);
            Assert.Equal("IType-0.TestMethod1 [a1,a2,a3]", agentCommand.Name);

        }
        [Fact]
        public void AgentCommandRegisterTest1()
        {

            Utility.Command command = new Utility.Command();
            int regCount = 0;
            command.RegisterEvent += (cmd, ret, args) =>
            {
                regCount++;
            };
            int unregCount = 0;
            command.UnregisterEvent += (cmd) =>
            {
                unregCount++;
            };
            Remote.Client.AgentCommandRegister agentCommandRegister = new Remote.Client.AgentCommandRegister(command, new Remote.Client.TypeConverterSet());

            CType test = new CType(1);

            agentCommandRegister.Regist(typeof(IType), test);
            agentCommandRegister.Unregist(test);

            Assert.Equal(2, regCount);
            Assert.Equal(2, unregCount);
        }

    }
}