using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests
{
    [TestClass ]
    public class ConnectTest
    {
        [TestMethod ]
        public void Test()
        {
            var hostEndpoint = new IPEndPoint(IPAddress.Parse("0.0.0.1") , 0);
            var agentEndpoint = new IPEndPoint(IPAddress.Parse("0.0.0.2"), 0);

            var hostSocket = new FakeSocket(hostEndpoint);            
            var agentSocket = new FakeSocket(agentEndpoint);

            hostSocket.SendEvent += (pkg) =>
            {
                var package = new SocketPackage();
                package.EndPoint = hostEndpoint;
                package.Segment = pkg.Segment;
                agentSocket.Receive(package);
            };
            agentSocket.SendEvent += (pkg) =>
            {
                var package = new SocketPackage();
                package.EndPoint = agentEndpoint;
                package.Segment = pkg.Segment;
                hostSocket.Receive(package);
            };

            var host = new Regulus.Network.RUDP.Host(hostSocket, hostSocket);
            var agent = new Regulus.Network.RUDP.Agent(agentSocket, agentSocket);
            var clientPeer = agent.Connect(hostEndpoint);

            var updater = new Updater<Timestamp>();
            updater.Add(hostSocket);
            updater.Add(agentSocket);
            updater.Add(host);
            updater.Add(agent);
            
            long ticks = 0;

            bool result = false;                        

            IPeer peer = null;
            host.AcceptEvent += p => peer = p;

            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));



            Assert.AreNotEqual(null , peer);
            Assert.AreEqual(PEER_STATUS.TRANSMISSION, clientPeer.Status);


            var sendBuffer = new byte[] {1, 2, 3, 4, 5};
            clientPeer.Send(sendBuffer);
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));


            var receivedBuffer = peer.Receive();


            

            updater.Working(new Timestamp(ticks++, 1));

            Assert.AreEqual(sendBuffer.Length , receivedBuffer.Length);

            
            clientPeer.Disconnect();


            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));


            Assert.AreEqual(PEER_STATUS.CLOSE , peer.Status);



        }

       
    }
}
