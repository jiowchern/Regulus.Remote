using NUnit.Framework;
using Regulus.Integration.Tests.SimulateReals;
using Regulus.Remote;
using Regulus.Remote.Ghost;
using Regulus.Remote.Tools.Protocol.Sources.TestCommon;
using System.Reflection.PortableExecutable;

namespace Regulus.Integration.Tests
{
    namespace SimulateReals
    {
        

        
        public class SimulateRealEnvironmentTests
        {
            [Test]
            public void ConnectTest()
            {
                IProtocol protocol = Regulus.Remote.Tools.Protocol.Sources.TestCommon.ProtocolProvider.CreateCase1();
                var port = Regulus.Network.Tcp.Tools.GetAvailablePort();
                var entry = new Server.Entry();

                var set = Regulus.Remote.Server.Provider.CreateTcpService(entry, protocol);
                set.Listener.Bind(port);
                

                var clients = new System.Collections.Generic.List<Client.User>();
                for (int i = 0; i < 10; i++)
                {
                    var client = new Client.User(i+1 ,port, protocol);
                    clients.Add(client);
                }

                System.Threading.Tasks.Parallel.ForEach(clients, client =>
                {
                    while(!client.IsDone())
                    {
                        client.Update();
                    }
                    
                });


                set.Service.Dispose();
            }
        }
    }
    
}