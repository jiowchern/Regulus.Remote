using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Regulus.Network;
using System.Linq;

namespace Regulus.Remote.Standalone.Test
{
    public static class SerializerHelper
    {
        public static byte[] ServerToClient<T>(this Regulus.Serialization.ISerializer serializer, ServerToClientOpCode opcode, T instance)
        {
            var pkg = new Regulus.Remote.ResponsePackage() { Code = opcode, Data = serializer.Serialize(instance) };
            return serializer.Serialize(pkg);
        }

        public static void ServerToClient<T>(this Regulus.Remote.PackageWriter<ResponsePackage> writer, Regulus.Serialization.ISerializer serializer, ServerToClientOpCode opcode, T instance)
        {
            ResponsePackage pkg = new ResponsePackage();
            pkg.Code = opcode;
            pkg.Data = serializer.Serialize(instance);
            writer.Push(new[] { pkg });            
        }
    }
    public class StandaloneTest1
    {

        [Test]
        public void CommunicationDevicePushTest()
        {
            var sendBuf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var recvBuf = new byte[10] ;

            var cd = new Regulus.Remote.Standalone.CommunicationDevice();
            var peer = cd as IPeer;
            cd.Push(sendBuf,0, sendBuf.Length);
            var receiveResult1 = peer.Receive(recvBuf, 0, 4);
            var receiveResult2 = peer.Receive(recvBuf, 4, 6);

            var receiveCount1 = receiveResult1.GetAwaiter().GetResult();            
            var receiveCount2 = receiveResult2.GetAwaiter().GetResult();

            Assert.AreEqual(4 , receiveCount1);
            Assert.AreEqual(6, receiveCount2);
        }

        [Test]
        public void CommunicationDevicePopTest()
        {
            var sendBuf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var recvBuf = new byte[10] ;

            var cd = new Regulus.Remote.Standalone.CommunicationDevice();
            var peer = cd as IPeer;
            
            var result1 = peer.Send(sendBuf, 0, 4);
            var sendResult1 = result1.GetAwaiter().GetResult();

            var result2 = peer.Send(sendBuf, 4, 6);
            var sendResult2 = result2.GetAwaiter().GetResult();


            var streamTask1 = cd.Pop(recvBuf,0,3);
            var stream1 = streamTask1.GetAwaiter().GetResult();

            var streamTask2 = cd.Pop(recvBuf, stream1, recvBuf.Length - stream1);
            var stream2 = streamTask2.GetAwaiter().GetResult();

            var streamTask3 = cd.Pop(recvBuf, stream1+stream2, recvBuf.Length - (stream1 + stream2));
            var stream3 = streamTask3.GetAwaiter().GetResult();



            Assert.AreEqual(10, stream3 + stream2+ stream1);
            Assert.AreEqual((byte)0, recvBuf[0]);
            Assert.AreEqual((byte)1, recvBuf[1]);
            Assert.AreEqual((byte)2, recvBuf[2]);
            Assert.AreEqual((byte)3, recvBuf[3]);
            Assert.AreEqual((byte)4, recvBuf[4]);
            Assert.AreEqual((byte)5, recvBuf[5]);
            Assert.AreEqual((byte)6, recvBuf[6]);
            Assert.AreEqual((byte)7, recvBuf[7]);
            Assert.AreEqual((byte)8, recvBuf[8]);
            Assert.AreEqual((byte)9, recvBuf[9]);

        }
        [Test]
        public void CommunicationDeviceSerializerTest()
        {
            Regulus.Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            var cd = new Regulus.Remote.Standalone.CommunicationDevice();
            IPeer peer = cd;
            var buf = serializer.ServerToClient(ServerToClientOpCode.LoadSoul, new Regulus.Remote.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });
            cd.Push(buf,0,buf.Length);

            var recvBuf = new byte[buf.Length];
            peer.Receive(recvBuf, 0, recvBuf.Length);
            var responsePkg = serializer.Deserialize(recvBuf) as ResponsePackage;
            var lordsoulPkg = serializer.Deserialize(responsePkg.Data)  as PackageLoadSoul;
            Assert.AreEqual(ServerToClientOpCode.LoadSoul , responsePkg.Code);
            Assert.AreEqual(1, lordsoulPkg.EntityId);
            Assert.AreEqual(false, lordsoulPkg.ReturnType);
            Assert.AreEqual(1, lordsoulPkg.TypeId);
        }

        [Test]
        public void CommunicationDeviceSerializerBatchTest()
        {
            Regulus.Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            var cd = new Regulus.Remote.Standalone.CommunicationDevice();
            IPeer peer = cd;

            var buf = serializer.ServerToClient(ServerToClientOpCode.LoadSoul, new Regulus.Remote.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });

            cd.Push(buf,0,1);
            cd.Push(buf,1,buf.Length - 1);

            var recvBuf = new byte[buf.Length];
            peer.Receive(recvBuf, 0, recvBuf.Length).GetAwaiter().GetResult();
            peer.Receive(recvBuf, 1, recvBuf.Length-1).GetAwaiter().GetResult();
            var responsePkg = serializer.Deserialize(recvBuf) as ResponsePackage;
            var lordsoulPkg = serializer.Deserialize(responsePkg.Data) as PackageLoadSoul;
            Assert.AreEqual(ServerToClientOpCode.LoadSoul, responsePkg.Code);
            Assert.AreEqual(1, lordsoulPkg.EntityId);
            Assert.AreEqual(false, lordsoulPkg.ReturnType);
            Assert.AreEqual(1, lordsoulPkg.TypeId);
        }
        [Test]
        [Timeout(10000)]
        public void AgentSupplyGpiTest()
        {
            IGpiA retGpiA = null;
            Regulus.Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            IProtocol protocol = NSubstitute.Substitute.For<IProtocol>();
            var types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IGpiA)  , typeof(CIGpiA));
            InterfaceProvider interfaceProvider = new InterfaceProvider(types) ;
            protocol.GetInterfaceProvider().Returns(interfaceProvider );
            protocol.GetSerialize().Returns(serializer);
            System.Func<IProvider> gpiaProviderProvider = () => new TProvider<IGpiA>();
            var typeProviderProvider = new System.Tuple<System.Type, System.Func<IProvider>>[] { new System.Tuple<System.Type, System.Func<IProvider>>(typeof(IGpiA) , gpiaProviderProvider) };
            protocol.GetMemberMap().Returns(new MemberMap(new System.Reflection.MethodInfo[0] , new System.Reflection.EventInfo[0] , new System.Reflection.PropertyInfo[0] , typeProviderProvider ));

            var cdClient = new Regulus.Remote.Standalone.CommunicationDevice();
            
            Network.IPeer peerClient = cdClient;
            var writer = new PackageWriter<ResponsePackage>(serializer);
            writer.Start(new ReversePeer(cdClient)) ;
            
            var agent = new Regulus.Remote.Ghost.Agent(protocol) as Ghost.IAgent;
            agent.QueryNotifier<IGpiA>().Supply+= gpi => retGpiA = gpi;
            agent.Start(peerClient);

            writer.ServerToClient(serializer,ServerToClientOpCode.LoadSoul, new Regulus.Remote.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });
            writer.ServerToClient(serializer,ServerToClientOpCode.LoadSoulCompile, new Regulus.Remote.PackageLoadSoulCompile() { EntityId = 1,  TypeId = 1 , ReturnId = 0 , PassageId = 0});
            while (retGpiA == null)
            {
                agent.Update();
            }
            agent.Stop();
            writer.Stop();
            Assert.AreNotEqual(null, retGpiA);
        }
    }
}
