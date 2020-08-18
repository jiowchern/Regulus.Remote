using NUnit.Framework;

namespace Regulus.Remote.Standalone.Test
{
    public class StandaloneTest
    {

        /*[Test]
        [Timeout(6000000)]
        public void DebugTestLoop()
        {
            
            for (int i = 0; i < 1000; i++)
            {
                Test();
            }
        }*/
            [Test]
        [Timeout(10000)]
        public void Test()
        {
            IBinderProvider entry = NSubstitute.Substitute.For<IBinderProvider>();
            IGpiA gpia = new SoulGpiA();
            bool bind = false;
            entry.AssignBinder(NSubstitute.Arg.Do<IBinder>(binder => {
                bind = true;
                binder.Bind<IGpiA>(gpia);                
            }));

            Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol(serializer);

            var service = Regulus.Remote.Standalone.Provider.CreateService(protocol, entry);
            Ghost.IAgent agent = new Regulus.Remote.Ghost.Agent(protocol);
            service.Join(agent);
            IGpiA retGpiA = null;
            agent.QueryNotifier<IGpiA>().Supply += gpi => retGpiA = gpi;
            while (retGpiA == null)
                agent.Update();
            service.Leave(agent);
            service.Dispose();

            Assert.AreNotEqual(null, retGpiA);
            Assert.AreEqual(true, bind);

        }

    }
}
