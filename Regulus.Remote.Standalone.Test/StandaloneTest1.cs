using NSubstitute;
using NUnit.Framework;
using Regulus.Network;

namespace Regulus.Remote.Standalone.Test
{
    public class StandaloneTest1
    {

        [Test]
        public void CommunicationDevicePushTest()
        {
            var sendBuf = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var recvBuf = new byte[10] ;
            

            var cd = new Regulus.Remote.Standalone.CommunicationDevice();
            var peer = cd as IPeer;
            cd.Push(sendBuf);
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

            var cd = new Regulus.Remote.Standalone.CommunicationDevice();
            var peer = cd as IPeer;
            
            var result1 = peer.Send(sendBuf, 0, 4);
            var sendResult1 = result1.GetAwaiter().GetResult();

            var result2 = peer.Send(sendBuf, 4, 6);
            var sendResult2 = result2.GetAwaiter().GetResult();


            var streamTask1 = cd.Pop();
            var stream1 = streamTask1.GetAwaiter().GetResult();

            var streamTask2 = cd.Pop();
            var stream2 = streamTask2.GetAwaiter().GetResult();

            Assert.AreEqual(sendResult1, stream1.Length - stream1.Position);
            Assert.AreEqual(sendResult2, stream2.Length - stream2.Position);
        }
        [Test]
        [Timeout(10000)]
        public void FullTest()
        {
            IGpiA retGpiA = null;
            
            IProtocol protocol = NSubstitute.Substitute.For<IProtocol>();
            Network.IPeer peer = NSubstitute.Substitute.For<Network.IPeer>();

            var agent = new Regulus.Remote.Ghost.Agent(protocol, peer) as IAgent;
            agent.QueryNotifier<IGpiA>();
            agent.Launch();
            while(retGpiA == null)
            {

            }
            agent.Shutdown();

            Assert.AreNotEqual(null, retGpiA);
        }
    }
}