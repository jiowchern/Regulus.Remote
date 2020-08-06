using NUnit.Framework;

namespace Regulus.Remote.Standalone.Test
{
    public class StandaloneTest
    {
        [Test]
        [Timeout(10000)]
        public void StandaloneNewTest()
        {
            IBinderProvider entry = NSubstitute.Substitute.For<IBinderProvider>();
            IGpiA gpia = new SoulGpiA();
            entry.AssignBinder(NSubstitute.Arg.Do<IBinder>(binder => binder.Bind<IGpiA>(gpia)));

            Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol(serializer);

            var service = new Regulus.Remote.Standalone.Service(entry , protocol) ;
            INotifierQueryable queryable = service.CreateNotifierQueryer();
            IGpiA retGpiA = null;
            queryable.QueryNotifier<IGpiA>().Supply += gpi => retGpiA = gpi;
            while(retGpiA == null)
                service.Update();
            service.DestroyNotifierQueryer(queryable);
            System.IDisposable disposable = service;
            disposable.Dispose();
            Assert.AreNotEqual(null , retGpiA);
        }
    }
}
