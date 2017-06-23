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
    [TestClass]
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
                package.Buffer = pkg.Buffer;
                agentSocket.Receive(package);
            };
            agentSocket.SendEvent += (pkg) =>
            {
                var package = new SocketPackage();
                package.EndPoint = agentEndpoint;
                package.Buffer = pkg.Buffer;
                hostSocket.Receive(package);
            };

            var host = new Regulus.Network.RUDP.Host(hostSocket, hostSocket);
            var agent = new Regulus.Network.RUDP.Agent(agentSocket, agentSocket);


            var updater = new Updater<Timestamp>();
            updater.Add(hostSocket);
            updater.Add(agentSocket);
            updater.Add(host);
            updater.Add(agent);
            
            long ticks = 0;

            bool result = false;
            agent.ConnectResultEvent += r => result = r;
            agent.Connect(hostEndpoint);

            IPeer peer = null;
            host.AcceptEvent += p => peer = p;

            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));


            Assert.AreNotEqual(null , peer);
            Assert.AreEqual(true, result);


            var sendBuffer = new byte[] {1, 2, 3, 4, 5};
            var receivedBuffer = new byte[0];

            peer.ReceivedEvent += pkg => receivedBuffer = pkg;
            agent.Send(sendBuffer);

            updater.Working(new Timestamp(ticks++, 1));

            Assert.AreEqual(sendBuffer.Length , receivedBuffer.Length);
        }

       
    }
}
