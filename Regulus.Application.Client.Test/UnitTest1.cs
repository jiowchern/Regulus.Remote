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
            var types = new System.Collections.Generic.Dictionary<System.Type, System.Type>() ;
            types.Add(typeof(IType) , typeof(CType));
            var ip = new InterfaceProvider(types);
            var type = ip.Types.First();

            Assert.AreEqual(type , typeof(IType));


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
                
                rectifier.SupplyEvent += (type, instance) => {
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
                rectifier.UnsupplyEvent += (type, instance) => {
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
        public void CommandAgentEventRectifierUnsupplyTest()
        {
            

            var types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IType), typeof(CType));
            var ip = new InterfaceProvider(types);


            TestAgent agent = new TestAgent();
            var cType = new CType(System.Guid.NewGuid());

            
            using (var rectifier = new Remote.Client.AgentEventRectifier(ip.Types, agent))
            {
                var command = new Regulus.Utility.Command();

                var agentCommandRegist = new Remote.Client.AgentCommandBinder(command, rectifier );
            }


        }
    }
}