using NUnit.Framework;
using Regulus.Network;
using System;

namespace Regulus.Remote.Standalone.Test
{
    public class SoulTest
    {
        [Test]
        [Timeout(10000)]
        public void ServiceNewTest()
        {
            var serverPeerStream = new Regulus.Remote.Standalone.PeerStream();
            IStreamable serverStream = serverPeerStream;
            IStreamable clientStream = new ReversePeer(serverPeerStream);


            IBinderProvider entry = NSubstitute.Substitute.For<IBinderProvider>();
            IGpiA gpia = new SoulGpiA();
            entry.AssignBinder(NSubstitute.Arg.Do<IBinder>( binder=> binder.Bind<IGpiA>(gpia) ));

            Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol(serializer);
            var service = new Regulus.Remote.Soul.Service(entry, protocol);
            var agent = new Regulus.Remote.Ghost.Agent(protocol) as Ghost.IAgent;
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
