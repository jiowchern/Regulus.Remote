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
            var  listenable = NSubstitute.Substitute.For<Soul.IListenable>();
            IGpiA gpia = new SoulGpiA();
            entry.AssignBinder(NSubstitute.Arg.Do<IBinder>(binder => binder.Bind<IGpiA>(gpia)));

            ISerializable serializer = new Regulus.Remote.DynamicSerializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol();
            var internalSer = new Regulus.Remote.InternalSerializer();
            Soul.IService service = new Regulus.Remote.Soul.Service(entry, protocol, serializer , listenable, internalSer);
            IAgent agent = new Regulus.Remote.Ghost.Agent(clientStream,protocol, serializer, internalSer) as Ghost.IAgent;
            IGpiA ghostGpia = null;


            listenable.StreamableEnterEvent += NSubstitute.Raise.Event<System.Action<IStreamable>>(serverStream);            
            
            agent.QueryNotifier<IGpiA>().Supply += gpi => ghostGpia = gpi;

            while (ghostGpia == null)
            {
                agent.Update();
            }

            agent.Dispose();
            listenable.StreamableLeaveEvent += NSubstitute.Raise.Event<System.Action<IStreamable>>(serverStream);

            IDisposable disposable = service;
            disposable.Dispose();
        }
    }
}
