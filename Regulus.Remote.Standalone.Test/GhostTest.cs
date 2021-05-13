using NSubstitute;
using Xunit;
using System.Linq;
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
            var types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
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
        //[Fact()]    
        public void  CommunicationDevicePushTestMutli()
        {
            var tasks = from _ in System.Linq.Enumerable.Range(0, 10000000)
                        select CommunicationDevicePushTest();

            System.Threading.Tasks.Task.WhenAll(tasks);


            

        }
        [Fact(/*Timeout = 5000*/)]
        public async System.Threading.Tasks.Task CommunicationDevicePushTest()
        {
            byte[] sendBuf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] recvBuf = new byte[10];

            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd as IStreamable;
            await cd.Push(sendBuf, 0, sendBuf.Length);
            var receiveResult1 = peer.Receive(recvBuf, 0, 4);
            var receiveResult2 = peer.Receive(recvBuf, 4, 5);
            var receiveResult3 = peer.Receive(recvBuf, 9, 2);

            int receiveCount1 = receiveResult1.GetAwaiter().GetResult();
            int receiveCount2 = receiveResult2.GetAwaiter().GetResult();
            int receiveCount3 = receiveResult3.GetAwaiter().GetResult();

            Assert.Equal(4, receiveCount1);
            Assert.Equal(5, receiveCount2);
            Assert.Equal(1, receiveCount3);
        }

        [Fact(Timeout =5000)]        
        public async void CommunicationDevicePopTest()
        {
            byte[] sendBuf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] recvBuf = new byte[10];

            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd as IStreamable;

            var result1 = peer.Send(sendBuf, 0, 4);
            int sendResult1 = await result1;

            var result2 = peer.Send(sendBuf, 4, 6);
            int sendResult2 = await result2;


            System.Threading.Tasks.Task<int> streamTask1 = cd.Pop(recvBuf, 0, 3);
            int stream1 = await streamTask1;

            System.Threading.Tasks.Task<int> streamTask2 = cd.Pop(recvBuf, stream1, recvBuf.Length - stream1);
            int stream2 = await streamTask2;

            System.Threading.Tasks.Task<int> streamTask3 = cd.Pop(recvBuf, stream1 + stream2, recvBuf.Length - (stream1 + stream2));
            int stream3 = await streamTask3;



            Assert.Equal(10, stream3 + stream2 + stream1);
            Assert.Equal((byte)0, recvBuf[0]);
            Assert.Equal((byte)1, recvBuf[1]);
            Assert.Equal((byte)2, recvBuf[2]);
            Assert.Equal((byte)3, recvBuf[3]);
            Assert.Equal((byte)4, recvBuf[4]);
            Assert.Equal((byte)5, recvBuf[5]);
            Assert.Equal((byte)6, recvBuf[6]);
            Assert.Equal((byte)7, recvBuf[7]);
            Assert.Equal((byte)8, recvBuf[8]);
            Assert.Equal((byte)9, recvBuf[9]);

        }
        [Fact(Timeout =5000)]        
        public async void CommunicationDeviceSerializerTest()
        {
            Regulus.Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd;
            byte[] buf = serializer.ServerToClient(ServerToClientOpCode.LoadSoul, new Regulus.Remote.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });
            await cd.Push(buf, 0, buf.Length);

            byte[] recvBuf = new byte[buf.Length];
            await peer.Receive(recvBuf, 0, recvBuf.Length);
            ResponsePackage responsePkg = serializer.Deserialize(recvBuf) as ResponsePackage;
            PackageLoadSoul lordsoulPkg = serializer.Deserialize(responsePkg.Data) as PackageLoadSoul;
            Assert.Equal(ServerToClientOpCode.LoadSoul, responsePkg.Code);
            Assert.Equal(1, lordsoulPkg.EntityId);
            Assert.False(lordsoulPkg.ReturnType);
            Assert.Equal(1, lordsoulPkg.TypeId);
        }

        [Fact(Timeout  = 5000)]
        
        public  async void CommunicationDeviceSerializerBatchTest()
        {
            Regulus.Serialization.ISerializer serializer = new Regulus.Serialization.Dynamic.Serializer();
            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd;

            byte[] buf = serializer.ServerToClient(ServerToClientOpCode.LoadSoul, new Regulus.Remote.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });

            await cd.Push(buf, 0, 1);
            await cd.Push(buf, 1, buf.Length - 1);

            byte[] recvBuf = new byte[buf.Length];
            await peer.Receive(recvBuf, 0, recvBuf.Length);
            await peer.Receive(recvBuf, 1, recvBuf.Length - 1);
            ResponsePackage responsePkg = serializer.Deserialize(recvBuf) as ResponsePackage;
            PackageLoadSoul lordsoulPkg = serializer.Deserialize(responsePkg.Data) as PackageLoadSoul;
            Assert.Equal(ServerToClientOpCode.LoadSoul, responsePkg.Code);
            Assert.Equal(1, lordsoulPkg.EntityId);
            Assert.False(lordsoulPkg.ReturnType);
            Assert.Equal(1, lordsoulPkg.TypeId);
        }
        [Fact(Timeout =10000)]        
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
            writer.ServerToClient(serializer, ServerToClientOpCode.LoadSoulCompile, new Regulus.Remote.PackageLoadSoulCompile() { EntityId = 1, TypeId = 1, ReturnId = 0});
            while (retGpiA == null)
            {
                agent.Update();
                writer.Update();
            }
            agent.Stop();
            writer.Stop();
            Assert.NotNull(retGpiA);
        }


    }
}
