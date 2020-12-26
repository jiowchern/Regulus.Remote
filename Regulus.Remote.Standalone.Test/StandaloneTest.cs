using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Regulus.Remote.Standalone.Test
{

    public class TestTwoEventsReceivedEntry : IBinderProvider
    {
        void IBinderProvider.AssignBinder(IBinder binder, object state)
        {
            throw new System.NotImplementedException();
        }
    }

    public class StandaloneTest
    {

        
        [Test]
        public void Test()
        {
            IBinderProvider entry = NSubstitute.Substitute.For<IBinderProvider>();
            IGpiA gpia = new SoulGpiA();
            bool bind = false;
            entry.AssignBinder(NSubstitute.Arg.Do<IBinder>(binder =>
            {
                bind = true;
                binder.Bind<IGpiA>(gpia);
            }), NSubstitute.Arg.Any<object>());

            Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol(serializer);
            
            _TestService(entry,ref bind, protocol);

        }

        [Test]
        public void TestTwoEventsReceived()
        {
            var entry = new TestTwoEventsReceivedEntry();
            Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol(serializer);
            IService service = new Regulus.Remote.Standalone.Service( entry , protocol);
            Ghost.IAgent agent = new Regulus.Remote.Ghost.Agent(protocol);
            service.Join(agent);
            
            /*while (!entry.IsDone())
            {
                agent.Update();
            }*/
            
            service.Leave(agent);
            service.Dispose();

        //    NUnit.Framework.Assert.AreEqual();
        }

        private static void _TestService(IBinderProvider entry,ref bool bind, IProtocol protocol)
        {
            bind = false;
            var service = Regulus.Remote.Standalone.Provider.CreateService(protocol, entry);
            Ghost.IAgent agent = new Regulus.Remote.Ghost.Agent(protocol);
            service.Join(agent, null);
            IGpiA retGpiA = null;
            bool ret = false;
            agent.QueryNotifier<IGpiA>().Supply += gpi => {
                ret = true;
                retGpiA = gpi; 
            };
            var timer = new Regulus.Utility.TimeCounter();
            var apr = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
            while (retGpiA == null)
            {
                apr.Operate();
                agent.Update();
                if (timer.Second > 10)
                {

                    throw new System.Exception($"debug agent:{agent.Active} bind:{bind} ");
                }

            }

            service.Leave(agent);
            service.Dispose();

            Assert.AreNotEqual(null, retGpiA);
            Assert.AreEqual(true, bind);
            Assert.AreEqual(true, ret);

        }
    }
}
