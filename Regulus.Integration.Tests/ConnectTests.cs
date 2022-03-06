using Regulus.Remote;
using NUnit.Framework;
using System.Linq;
using System.Reactive.Linq;
using Regulus.Remote.Reactive;
using NSubstitute;
namespace Regulus.Integration.Tests
{
    public class ConnectTests
    {
        [Test]
        public void TcpLocalConnectTest()
        {
            // bind interface
            var tester = new Regulus.Remote.Tools.Protocol.Sources.TestCommon.MethodTester();
            var entry = NSubstitute.Substitute.For<IEntry>();            
            entry.AssignBinder(NSubstitute.Arg.Do<IBinder>(b=>b.Bind<Regulus.Remote.Tools.Protocol.Sources.TestCommon.IMethodable>(tester) ));

            // create protocol
            IProtocol protocol = Regulus.Remote.Tools.Protocol.Sources.TestCommon.ProtocolProvider.CreateCase1();

            // create server and client
            var server = Regulus.Remote.Server.Provider.CreateTcpService(entry, protocol);
            server.Listener.Bind(47536);
            
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
            System.Net.IPEndPoint.TryParse("127.0.0.1:47536", out endPoint);
            client.Connecter.Connect(endPoint).Wait();

            // get values
            var valuesObs = from gpi in client.Agent.QueryNotifier<Regulus.Remote.Tools.Protocol.Sources.TestCommon.IMethodable>().SupplyEvent()
                            from v1 in gpi.GetValue1().RemoteValue()
                            from v2 in gpi.GetValue2().RemoteValue()
                            from v0 in gpi.GetValue0(0, "", 0, 0, 0, System.Guid.Empty).RemoteValue()
                            select new { v1, v2, v0 };


            var values = valuesObs.FirstAsync().Wait();
            stop = true;
            task.Wait();

            // release
            client.Connecter.Disconnect();
            client.Agent.Dispose();

            server.Listener.Close();
            server.Service.Dispose();

            // test
            Assert.AreEqual(1, values.v1);
            Assert.AreEqual(2, values.v2);
            Assert.AreEqual(0, values.v0);
        }
    }
}