using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests
{
    
    public class ConnectTest
    {
        [Test]
        public void TestFullFlow()
        {
            ISocketPackageSpawner spawner = SocketPackagePool.Instance;
            var hostEndpoint = new IPEndPoint(IPAddress.Parse("0.0.0.1") , 0);
            var agentEndpoint = new IPEndPoint(IPAddress.Parse("0.0.0.2"), 0);

            var hostSocket = new FakeSocket(hostEndpoint);            
            var agentSocket = new FakeSocket(agentEndpoint);

            hostSocket.SendEvent += (pkg) =>
            {
                var package = spawner.Spawn();
                package.SetEndPoint(hostEndpoint);
                Buffer.BlockCopy(pkg.Package, 0, package.Package, 0, pkg.Package.Length);
                
                agentSocket.Receive(package);
            };
            agentSocket.SendEvent += (pkg) =>
            {
                var package = spawner.Spawn();
                package.SetEndPoint(agentEndpoint);
                Buffer.BlockCopy(pkg.Package, 0, package.Package, 0, pkg.Package.Length);
                
                hostSocket.Receive(package);
            };

            var host = new Regulus.Network.RUDP.Host(hostSocket, hostSocket);
            var agent = new Regulus.Network.RUDP.Agent(agentSocket, agentSocket);
            var clientPeer = agent.Connect(hostEndpoint,(connect_result) =>{});

            var updater = new Updater<Timestamp>();
            updater.Add(hostSocket);
            updater.Add(agentSocket);
            updater.Add(host);
            updater.Add(agent);
            
            long ticks = 0;
            

            IRudpPeer rudpPeer = null;
            host.AcceptEvent += p => rudpPeer = p;

            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));

            Assert.AreNotEqual(null , rudpPeer);
            Assert.AreEqual(PEER_STATUS.TRANSMISSION, clientPeer.Status);


            var sendBuffer = new byte[] {1, 2, 3, 4, 5};
            clientPeer.Send(sendBuffer,0, sendBuffer.Length , (send_count , error)=>{});
            

            int readCount = 0;
            var receivedBuffer = new byte[Config.Default.PackageSize];
            rudpPeer.Receive(receivedBuffer, 0, receivedBuffer.Length, (read_count, error) =>
            {
                readCount = read_count;
            });

            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));

            Assert.AreEqual(sendBuffer.Length , readCount);

            
            clientPeer.Disconnect();


            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));

            Assert.AreEqual(PEER_STATUS.CLOSE , rudpPeer.Status);



        }

       
    }
}
