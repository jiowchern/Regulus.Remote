using NSubstitute;
using NUnit.Framework;
using Regulus.Network;
using Regulus.Serialization;

namespace Regulus.Remote.Standalone.Test
{
    public static class SerializerHelper
    {
        public static byte[] ServerToClient<T>(this Regulus.Serialization.ISerializer serializer, ServerToClientOpCode opcode, T instance)
        {
            ResponsePackage pkg = new Regulus.Remote.ResponsePackage() { Code = opcode, Data = serializer.Serialize(instance) };
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

    public class ProtocolHelper
    {
        public static IProtocol CreateProtocol(ISerializer serializer)
        {
            IProtocol protocol = NSubstitute.Substitute.For<IProtocol>();
            System.Collections.Generic.Dictionary<System.Type, System.Type> types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IGpiA), typeof(GhostIGpiA));
            InterfaceProvider interfaceProvider = new InterfaceProvider(types);
            protocol.GetInterfaceProvider().Returns(interfaceProvider);
            protocol.GetSerialize().Returns(serializer);
            System.Func<IProvider> gpiaProviderProvider = () => new TProvider<IGpiA>();
            System.Tuple<System.Type, System.Func<IProvider>>[] typeProviderProvider = new System.Tuple<System.Type, System.Func<IProvider>>[] { new System.Tuple<System.Type, System.Func<IProvider>>(typeof(IGpiA), gpiaProviderProvider) };
            protocol.GetMemberMap().Returns(new MemberMap(new System.Reflection.MethodInfo[0], new System.Reflection.EventInfo[0], new System.Reflection.PropertyInfo[0], typeProviderProvider));
            return protocol;
        }
    }
    public class GhostTest
    {

        [Test]
        public void CommunicationDevicePushTest()
        {
            byte[] sendBuf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] recvBuf = new byte[10];

            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd as IStreamable;
            cd.Push(sendBuf, 0, sendBuf.Length);
            System.Threading.Tasks.Task<int> receiveResult1 = peer.Receive(recvBuf, 0, 4);
            System.Threading.Tasks.Task<int> receiveResult2 = peer.Receive(recvBuf, 4, 6);

            int receiveCount1 = receiveResult1.GetAwaiter().GetResult();
            int receiveCount2 = receiveResult2.GetAwaiter().GetResult();

            Assert.AreEqual(4, receiveCount1);
            Assert.AreEqual(6, receiveCount2);
        }

        [Test]
        public void CommunicationDevicePopTest()
        {
            byte[] sendBuf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] recvBuf = new byte[10];

            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd as IStreamable;

            System.Threading.Tasks.Task<int> result1 = peer.Send(sendBuf, 0, 4);
            int sendResult1 = result1.GetAwaiter().GetResult();

            System.Threading.Tasks.Task<int> result2 = peer.Send(sendBuf, 4, 6);
            int sendResult2 = result2.GetAwaiter().GetResult();


            System.Threading.Tasks.Task<int> streamTask1 = cd.Pop(recvBuf, 0, 3);
            int stream1 = streamTask1.GetAwaiter().GetResult();

            System.Threading.Tasks.Task<int> streamTask2 = cd.Pop(recvBuf, stream1, recvBuf.Length - stream1);
            int stream2 = streamTask2.GetAwaiter().GetResult();

            System.Threading.Tasks.Task<int> streamTask3 = cd.Pop(recvBuf, stream1 + stream2, recvBuf.Length - (stream1 + stream2));
            int stream3 = streamTask3.GetAwaiter().GetResult();



            Assert.AreEqual(10, stream3 + stream2 + stream1);
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
            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd;
            byte[] buf = serializer.ServerToClient(ServerToClientOpCode.LoadSoul, new Regulus.Remote.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });
            cd.Push(buf, 0, buf.Length);

            byte[] recvBuf = new byte[buf.Length];
            peer.Receive(recvBuf, 0, recvBuf.Length);
            ResponsePackage responsePkg = serializer.Deserialize(recvBuf) as ResponsePackage;
            PackageLoadSoul lordsoulPkg = serializer.Deserialize(responsePkg.Data) as PackageLoadSoul;
            Assert.AreEqual(ServerToClientOpCode.LoadSoul, responsePkg.Code);
            Assert.AreEqual(1, lordsoulPkg.EntityId);
            Assert.AreEqual(false, lordsoulPkg.ReturnType);
            Assert.AreEqual(1, lordsoulPkg.TypeId);
        }

        [Test]
        public void CommunicationDeviceSerializerBatchTest()
        {
            Regulus.Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd;

            byte[] buf = serializer.ServerToClient(ServerToClientOpCode.LoadSoul, new Regulus.Remote.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });

            cd.Push(buf, 0, 1);
            cd.Push(buf, 1, buf.Length - 1);

            byte[] recvBuf = new byte[buf.Length];
            peer.Receive(recvBuf, 0, recvBuf.Length).GetAwaiter().GetResult();
            peer.Receive(recvBuf, 1, recvBuf.Length - 1).GetAwaiter().GetResult();
            ResponsePackage responsePkg = serializer.Deserialize(recvBuf) as ResponsePackage;
            PackageLoadSoul lordsoulPkg = serializer.Deserialize(responsePkg.Data) as PackageLoadSoul;
            Assert.AreEqual(ServerToClientOpCode.LoadSoul, responsePkg.Code);
            Assert.AreEqual(1, lordsoulPkg.EntityId);
            Assert.AreEqual(false, lordsoulPkg.ReturnType);
            Assert.AreEqual(1, lordsoulPkg.TypeId);
        }
        [Test]
        public void AgentSupplyGpiTest()
        {
            IGpiA retGpiA = null;
            Regulus.Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol(serializer);

            Stream cdClient = new Regulus.Remote.Standalone.Stream();

            Network.IStreamable peerClient = cdClient;
            PackageWriter<ResponsePackage> writer = new PackageWriter<ResponsePackage>(serializer);
            writer.Start(new ReverseStream(cdClient));

            Ghost.IAgent agent = new Regulus.Remote.Ghost.Agent(protocol) as Ghost.IAgent;
            agent.QueryNotifier<IGpiA>().Supply += gpi => retGpiA = gpi;
            agent.Start(peerClient);

            writer.ServerToClient(serializer, ServerToClientOpCode.LoadSoul, new Regulus.Remote.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });
            writer.ServerToClient(serializer, ServerToClientOpCode.LoadSoulCompile, new Regulus.Remote.PackageLoadSoulCompile() { EntityId = 1, TypeId = 1, ReturnId = 0, PassageId = 0 });
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
