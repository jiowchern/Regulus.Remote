using NUnit.Framework;

namespace Regulus.Remote.Standalone.Test
{
    public class StandaloneTest
    {
        [Test]
        [Timeout(10000)]
        public void Test()
        {
            IBinderProvider entry = NSubstitute.Substitute.For<IBinderProvider>();
            IGpiA gpia = new SoulGpiA();
            entry.AssignBinder(NSubstitute.Arg.Do<IBinder>(binder => binder.Bind<IGpiA>(gpia)));

            Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol(serializer);

            
            IService service = Regulus.Remote.Standalone.Provider.CreateService(protocol, entry);

            Ghost.IAgent agent = new Regulus.Remote.Ghost.Agent(protocol);
            service.Join(agent);
            IGpiA retGpiA = null;
            agent.QueryNotifier<IGpiA>().Supply += gpi => retGpiA = gpi;
            while (retGpiA == null)
                agent.Update();
            service.Leave(agent);
            System.IDisposable disposable = service;
            disposable.Dispose();
            Assert.AreNotEqual(null, retGpiA);
        }
    }
}
