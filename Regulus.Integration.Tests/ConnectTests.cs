using Regulus.Remote;
using NUnit.Framework;
using System.Linq;
using System.Reactive.Linq;
using Regulus.Remote.Reactive;
using NSubstitute;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
namespace Regulus.Integration.Tests
{
    public class ConnectTests
    {
        [Test]

        public async Task AgentDisconnectTest()
        {
            var port = Regulus.Network.Tcp.Tools.GetAvailablePort();
            var tester = new Regulus.Remote.Tools.Protocol.Sources.TestCommon.MethodTester();
            var entry = NSubstitute.Substitute.For<IEntry>();
            entry.RegisterClientBinder(NSubstitute.Arg.Do<IBinder>(b => b.Bind<Regulus.Remote.Tools.Protocol.Sources.TestCommon.IMethodable>(tester)));
            IProtocol protocol = Regulus.Remote.Tools.Protocol.Sources.TestCommon.ProtocolProvider.CreateCase1();

            var server = Regulus.Remote.Server.Provider.CreateTcpService(entry, protocol);
            server.Listener.Bind(port);

            var client = Regulus.Remote.Client.Provider.CreateTcpAgent(protocol);
            System.Exception ex;

            client.Agent.ExceptionEvent += (exc) => { 
                ex = exc; 
                NUnit.Framework.Assert.Pass(exc.ToString()); 
            };

            var peer = await client.Connector.Connect(new IPEndPoint(IPAddress.Loopback, port));
            SocketError error = SocketError.Success;
            peer.SocketErrorEvent += (e) => {
                error = e;
                NUnit.Framework.Assert.Pass(); 
            };

            client.Agent.Enable(peer);

            await client.Connector.Disconnect();

            while (error == SocketError.Success)
            {
                client.Agent.Update();
            }

            
        }
        [Test]
        public async Task TcpLocalConnectTest()
        {
            // 獲取一個臨時可用的端口
            int GetAvailablePort()
            {
                TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                listener.Stop();
                return port;
            }
            var port = GetAvailablePort();
            // bind interface
            var tester = new Regulus.Remote.Tools.Protocol.Sources.TestCommon.MethodTester();
            var entry = NSubstitute.Substitute.For<IEntry>();            
            entry.RegisterClientBinder(NSubstitute.Arg.Do<IBinder>(b=>b.Bind<Regulus.Remote.Tools.Protocol.Sources.TestCommon.IMethodable>(tester) ));

            // create protocol
            IProtocol protocol = Regulus.Remote.Tools.Protocol.Sources.TestCommon.ProtocolProvider.CreateCase1();

            // create server and client
            var server = Regulus.Remote.Server.Provider.CreateTcpService(entry, protocol);
            
            server.Listener.Bind(port);
            
            var client = Regulus.Remote.Client.Provider.CreateTcpAgent(protocol);

            bool stop = false;
            var task = System.Threading.Tasks.Task.Run(() => 
            {
                while (!stop)
                {
                    client.Agent.Update();
                }

            });

            // do connect
            System.Net.IPEndPoint endPoint;            
            System.Net.IPEndPoint.TryParse($"127.0.0.1:{port}", out endPoint);
            var peer = await client.Connector.Connect(endPoint);

            client.Agent.Enable(peer);
            // get values
            var valuesObs = from gpi in client.Agent.QueryNotifier<Regulus.Remote.Tools.Protocol.Sources.TestCommon.IMethodable>().SupplyEvent()
                            from v1 in gpi.GetValue1().RemoteValue()
                            from v2 in gpi.GetValue2().RemoteValue()
                            from v0 in gpi.GetValue0(0, "", 0, 0, 0, System.Guid.Empty).RemoteValue()
                            select new { v1, v2, v0 };


            var values = await valuesObs.FirstAsync();
            stop = true;
            await task;

            // release
            await client.Connector.Disconnect();
            client.Agent.Disable();

            server.Listener.Close();
            
            server.Service.Dispose();

            // test
            Assert.AreEqual(1, values.v1);
            Assert.AreEqual(2, values.v2);
            Assert.AreEqual(0, values.v0[0]);
        }
    }
}