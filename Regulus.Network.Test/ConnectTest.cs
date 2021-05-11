using Xunit;
using Regulus.Utility;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Regulus.Network.Tests
{
    public class ConnectTest
    {
        [Xunit.Fact(Timeout =5000)]
        
        public void TestFullFlow()
        {
            SocketMessageFactory spawner = SocketMessageFactory.Instance;
            IPEndPoint hostEndpoint = new IPEndPoint(IPAddress.Parse("0.0.0.1"), 0);
            IPEndPoint agentEndpoint = new IPEndPoint(IPAddress.Parse("0.0.0.2"), 0);

            FakeSocket hostSocket = new FakeSocket(hostEndpoint);
            FakeSocket agentSocket = new FakeSocket(agentEndpoint);

            hostSocket.SendEvent += (pkg) =>
            {
                Package.SocketMessage package = spawner.Spawn();
                package.SetEndPoint(hostEndpoint);
                Buffer.BlockCopy(pkg.Package, 0, package.Package, 0, pkg.Package.Length);

                agentSocket.Receive(package);
            };
            agentSocket.SendEvent += (pkg) =>
            {
                Package.SocketMessage package = spawner.Spawn();
                package.SetEndPoint(agentEndpoint);
                Buffer.BlockCopy(pkg.Package, 0, package.Package, 0, pkg.Package.Length);

                hostSocket.Receive(package);
            };

            Host host = new Regulus.Network.Host(hostSocket, hostSocket);
            Agent agent = new Regulus.Network.Agent(agentSocket, agentSocket);
            Socket clientPeer = agent.Connect(hostEndpoint, (connect_result) => { });

            Updater<Timestamp> updater = new Updater<Timestamp>();
            updater.Add(hostSocket);
            updater.Add(agentSocket);
            updater.Add(host);
            updater.Add(agent);

            long ticks = 0;


            Socket rudpSocket = null;
            host.AcceptEvent += p => rudpSocket = p;

            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));

            Assert.NotNull(rudpSocket);
            Assert.Equal(PeerStatus.Transmission, clientPeer.Status);


            byte[] sendBuffer = new byte[] { 1, 2, 3, 4, 5 };
            clientPeer.Send(sendBuffer, 0, sendBuffer.Length);


            int readCount = 0;
            byte[] receivedBuffer = new byte[Config.Default.PackageSize];
            Task<int> task = rudpSocket.Receive(receivedBuffer, 0, receivedBuffer.Length);







            Task waitTask = task.ContinueWith(t =>
            {
                readCount = t.Result;
            });

            while (readCount == 0)
            {
                updater.Working(new Timestamp(ticks++, 1));
            }

            Assert.Equal(sendBuffer.Length, readCount);

            clientPeer.Disconnect();



            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));
            updater.Working(new Timestamp(ticks++, 1));

            Assert.Equal(PeerStatus.Close, rudpSocket.Status);



        }


    }
}
