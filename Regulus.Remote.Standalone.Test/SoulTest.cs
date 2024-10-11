using NUnit.Framework;
using Regulus.Network;
using Regulus.Remote.Ghost;
using System;

namespace Regulus.Remote.Standalone.Test
{
    public class SoulTest
    {
        [NUnit.Framework.Test]        
        public void ServiceTest()
        {
            Stream serverPeerStream = new Regulus.Remote.Standalone.Stream();
            IStreamable serverStream = serverPeerStream;
            IStreamable clientStream = new ReverseStream(serverPeerStream);

            IEntry entry = NSubstitute.Substitute.For<IEntry>();
            var  listenable = NSubstitute.Substitute.For<Soul.IListenable>();
            IGpiA gpia = new SoulGpiA();
            entry.RegisterClientBinder(NSubstitute.Arg.Do<IBinder>(binder => binder.Bind<IGpiA>(gpia)));

            ISerializable serializer = new Regulus.Remote.DynamicSerializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol();
            var internalSer = new Regulus.Remote.InternalSerializer();
            var pool = Regulus.Memorys.PoolProvider.Shared;
            Soul.IService service = new Regulus.Remote.Soul.AsyncService(new Soul.SyncService(entry , new Soul.UserProvider(protocol, serializer, listenable, internalSer, pool)));

            
            var ghostAgent = new Regulus.Remote.Ghost.Agent(protocol, serializer, internalSer , pool) ;
            ghostAgent.Enable(clientStream);
            IAgent agent = ghostAgent;
            IGpiA ghostGpia = null;

            var wapper = NSubstitute.Raise.Event<System.Action<IStreamable>>(serverStream);
            listenable.StreamableEnterEvent += wapper;


            agent.QueryNotifier<IGpiA>().Supply += gpi => ghostGpia = gpi;

            while (ghostGpia == null)
            {
                agent.Update();
            }
            ghostAgent.Disable();
            agent.Disable();
            listenable.StreamableLeaveEvent -= wapper;
            
            IDisposable disposable = service;
            disposable.Dispose();
        }
    }
}
