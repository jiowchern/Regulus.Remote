using Xunit;
using Regulus.Network;
using Regulus.Remote.Ghost;
using System;

namespace Regulus.Remote.Standalone.Test
{
    public class SoulTest
    {
        [Xunit.Fact]        
        public void ServiceTest()
        {
            Stream serverPeerStream = new Regulus.Remote.Standalone.Stream();
            IStreamable serverStream = serverPeerStream;
            IStreamable clientStream = new ReverseStream(serverPeerStream);

            IBinderProvider entry = NSubstitute.Substitute.For<IBinderProvider>();
            IGpiA gpia = new SoulGpiA();
            entry.AssignBinder(NSubstitute.Arg.Do<IBinder>(binder => binder.Bind<IGpiA>(gpia)), NSubstitute.Arg.Any<object>());

            Serialization.ISerializable serializer = new Regulus.Serialization.Dynamic.Serializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol(serializer);
            Soul.IService service = new Regulus.Remote.Soul.Service(entry, protocol, serializer);
            IAgent agent = new Regulus.Remote.Ghost.Agent(protocol, serializer) as Ghost.IAgent;
            IGpiA ghostGpia = null;


            service.Join(serverStream);
            agent.Start(clientStream);
            agent.QueryNotifier<IGpiA>().Supply += gpi => ghostGpia = gpi;

            while (ghostGpia == null)
            {
                agent.Update();
            }

            agent.Stop();
            service.Leave(serverStream);

            IDisposable disposable = service;
            disposable.Dispose();
        }
    }
}
