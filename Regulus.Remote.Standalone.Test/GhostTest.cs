using NSubstitute;
using NUnit.Framework;
using System.Linq;
using Regulus.Network;
using Regulus.Serialization;

namespace Regulus.Remote.Standalone.Test
{
    public static class SerializerHelper
    {
        public static byte[] ServerToClient<T>(this Regulus.Remote.IInternalSerializable serializer, ServerToClientOpCode opcode, T instance)
        {
            Regulus.Remote.Packages.ResponsePackage pkg = new Regulus.Remote.Packages.ResponsePackage() { Code = opcode, Data = serializer.Serialize(instance) };
            return serializer.Serialize(pkg);
        }

        public static void ServerToClient<T>(this Regulus.Remote.PackageWriter<Regulus.Remote.Packages.ResponsePackage> writer, Regulus.Remote.IInternalSerializable serializer, ServerToClientOpCode opcode, T instance)
        {
            Regulus.Remote.Packages.ResponsePackage pkg = new Regulus.Remote.Packages.ResponsePackage();
            pkg.Code = opcode;
            pkg.Data = serializer.Serialize(instance);
            writer.Push( pkg );
        }
    }

    public class ProtocolHelper
    {
        public static IProtocol CreateProtocol()
        {
            IProtocol protocol = NSubstitute.Substitute.For<IProtocol>();
            var types = new System.Collections.Generic.Dictionary<System.Type, System.Type>();
            types.Add(typeof(IGpiA), typeof(GhostIGpiA));
            InterfaceProvider interfaceProvider = new InterfaceProvider(types);
            protocol.GetInterfaceProvider().Returns(interfaceProvider);
            
            System.Func<IProvider> gpiaProviderProvider = () => new TProvider<IGpiA>();
            System.Tuple<System.Type, System.Func<IProvider>>[] typeProviderProvider = new System.Tuple<System.Type, System.Func<IProvider>>[] { new System.Tuple<System.Type, System.Func<IProvider>>(typeof(IGpiA), gpiaProviderProvider) };
            protocol.GetMemberMap().Returns(new MemberMap(new System.Reflection.MethodInfo[0], new System.Reflection.EventInfo[0], new System.Reflection.PropertyInfo[0], typeProviderProvider));
            return protocol;
        }
    }
    public class GhostTest
    {
        //[Test()]    
        public void  CommunicationDevicePushTestMutli()
        {
            var tasks = from _ in System.Linq.Enumerable.Range(0, 10000000)
                        select CommunicationDevicePushTest();

            System.Threading.Tasks.Task.WhenAll(tasks);


            

        }
        [Test(/*Timeout = 5000*/)]
        public async System.Threading.Tasks.Task CommunicationDevicePushTest()
        {
            byte[] sendBuf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] recvBuf = new byte[10];

            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd as IStreamable;
            await cd.Push(sendBuf, 0, sendBuf.Length);
            

            int receiveCount1 =await peer.Receive(recvBuf, 0, 4);
            int receiveCount2 = await peer.Receive(recvBuf, 4, 5);
            int receiveCount3 = await peer.Receive(recvBuf, 9, 2);

            Assert.AreEqual(4, receiveCount1);
            Assert.AreEqual(5, receiveCount2);
            Assert.AreEqual(1, receiveCount3);
        }

        [Test(),Timeout(5000)]        
        public async System.Threading.Tasks.Task CommunicationDevicePopTest()
        {
            byte[] sendBuf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] recvBuf = new byte[10];

            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd as IStreamable;

            var result1 = peer.Send(sendBuf, 0, 4);
            int sendResult1 = await result1;

            var result2 = peer.Send(sendBuf, 4, 6);
            int sendResult2 = await result2;


            var streamTask1 = cd.Pop(recvBuf, 0, 3);
            int stream1 = await streamTask1;

            var streamTask2 = cd.Pop(recvBuf, stream1, recvBuf.Length - stream1);
            int stream2 = await streamTask2;

            var streamTask3 = cd.Pop(recvBuf, stream1 + stream2, recvBuf.Length - (stream1 + stream2));
            int stream3 = await streamTask3;



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
        [Test(), Timeout(5000)]
        public async System.Threading.Tasks.Task CommunicationDeviceSerializerTest()
        {
            var serializer = new Regulus.Remote.DynamicSerializer();
            IInternalSerializable internalSerializable = new InternalSerializer();
            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd;
            byte[] buf = internalSerializable.ServerToClient(ServerToClientOpCode.LoadSoul, new Regulus.Remote.Packages.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });
            await cd.Push(buf, 0, buf.Length);

            byte[] recvBuf = new byte[buf.Length];
            await peer.Receive(recvBuf, 0, recvBuf.Length);
            Regulus.Remote.Packages.ResponsePackage responsePkg = (Regulus.Remote.Packages.ResponsePackage)internalSerializable.Deserialize(recvBuf)  ;
            Regulus.Remote.Packages.PackageLoadSoul lordsoulPkg = (Regulus.Remote.Packages.PackageLoadSoul)internalSerializable.Deserialize(responsePkg.Data)  ;
            Assert.AreEqual(ServerToClientOpCode.LoadSoul, responsePkg.Code);
            Assert.AreEqual(1, lordsoulPkg.EntityId);
            Assert.False(lordsoulPkg.ReturnType);
            Assert.AreEqual(1, lordsoulPkg.TypeId);
        }

        [NUnit.Framework.Test(), NUnit.Framework.Timeout(5000)]

        public async System.Threading.Tasks.Task CommunicationDeviceSerializerBatchTest()
        {
            var serializer = new Regulus.Remote.DynamicSerializer();
            IInternalSerializable internalSerializable = new InternalSerializer();
            Stream cd = new Regulus.Remote.Standalone.Stream();
            IStreamable peer = cd;

            byte[] buf = internalSerializable.ServerToClient(ServerToClientOpCode.LoadSoul, new Regulus.Remote.Packages.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });

            await cd.Push(buf, 0, 1);
            await cd.Push(buf, 1, buf.Length - 1);

            byte[] recvBuf = new byte[buf.Length];
            await peer.Receive(recvBuf, 0, recvBuf.Length);
            await peer.Receive(recvBuf, 1, recvBuf.Length - 1);
            Regulus.Remote.Packages.ResponsePackage responsePkg = (Regulus.Remote.Packages.ResponsePackage)internalSerializable.Deserialize(recvBuf)  ;
            Regulus.Remote.Packages.PackageLoadSoul lordsoulPkg = (Regulus.Remote.Packages.PackageLoadSoul)internalSerializable.Deserialize(responsePkg.Data)  ;
            Assert.AreEqual(ServerToClientOpCode.LoadSoul, responsePkg.Code);
            Assert.AreEqual(1, lordsoulPkg.EntityId);
            Assert.False(lordsoulPkg.ReturnType);
            Assert.AreEqual(1, lordsoulPkg.TypeId);
        }
        [Test(), Timeout(5000)]
        public void AgentSupplyGpiTest()
        {
            IGpiA retGpiA = null;
            var serializer = new Regulus.Remote.DynamicSerializer();

            var internalSerializer = new InternalSerializer();
            IProtocol protocol = ProtocolHelper.CreateProtocol();

            Stream cdClient = new Regulus.Remote.Standalone.Stream();

            Network.IStreamable peerClient = cdClient;
            PackageWriter<Regulus.Remote.Packages.ResponsePackage> writer = new PackageWriter<Regulus.Remote.Packages.ResponsePackage>(internalSerializer);
            writer.Start(new ReverseStream(cdClient));

            Ghost.IAgent agent = new Regulus.Remote.Ghost.Agent(peerClient , protocol, serializer, internalSerializer) as Ghost.IAgent;
            agent.QueryNotifier<IGpiA>().Supply += gpi => retGpiA = gpi;
            

            writer.ServerToClient(internalSerializer, ServerToClientOpCode.LoadSoul, new Regulus.Remote.Packages.PackageLoadSoul() { EntityId = 1, ReturnType = false, TypeId = 1 });
            writer.ServerToClient(internalSerializer, ServerToClientOpCode.LoadSoulCompile, new Regulus.Remote.Packages.PackageLoadSoulCompile() { EntityId = 1, TypeId = 1, ReturnId = 0});
            var ar = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
            while (retGpiA == null)
            {
                ar.Operate();
                agent.Update();                
            }
            agent.Dispose();
            writer.Stop();
            Assert.NotNull(retGpiA);
        }


    }
}
