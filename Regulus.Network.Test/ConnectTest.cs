using System.Linq;
using System.Net.Sockets;

namespace Regulus.Network.Tests
{
    public class ConnectTest
    {
        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task ConnectSuccessTest()
        {
            var port = Regulus.Network.Tcp.Tools.GetAvailablePort();

            var lintener = new Regulus.Network.Tcp.Listener();
            lintener.AcceptEvent+= (peer) => { NUnit.Framework.Assert.IsNotNull(peer); };
            lintener.Bind(port);
            var connector = new Regulus.Network.Tcp.Connector();
            
            var peer = await connector.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port));
            
            NUnit.Framework.Assert.IsNotNull(peer);            

            if(false)
            {
                // disconnect test
                await connector.Disconnect(true);


                // reconnect test
                peer = await connector.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port));

                NUnit.Framework.Assert.IsNotNull(peer);
            }
            else
            {
                // disconnect test
                await connector.Disconnect(false);
            }

            lintener.Close();
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task ConnectFailTest()
        {
            var port = Regulus.Network.Tcp.Tools.GetAvailablePort();            

            var connector = new Regulus.Network.Tcp.Connector();
            var ex = await connector.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port)).ContinueWith(t => { 
                t.Exception.Handle(e =>
                {
                    NUnit.Framework.Assert.IsNotNull(e);
                    return true;
                });

                return t.Exception;
            });
            
            NUnit.Framework.Assert.IsNotNull(ex);
        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task DisconnectTest()
        {             
            var port = Regulus.Network.Tcp.Tools.GetAvailablePort();
        
            var lintener = new Regulus.Network.Tcp.Listener();
            var serverPeers = new System.Collections.Generic.List<Regulus.Network.Tcp.Peer>();

            bool breakEvent = false;
            lintener.AcceptEvent+= (peer) => 
            {
                peer.BreakEvent += () => { breakEvent = true; };
                
                serverPeers.Add(peer); 
            };
            lintener.Bind(port);
            var connector = new Regulus.Network.Tcp.Connector();
                   
            var peer = await connector.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port));

            {
                IStreamable streamable = peer;
                var buffer = new byte[1024];
                var count = await streamable.Send(buffer, 0, buffer.Length);
            }
            

            await connector.Disconnect(false);
            {
                IStreamable streamable = serverPeers.Single();
                var buffer = new byte[1024];
                var count = await streamable.Receive(buffer, 0, buffer.Length);
                var count2 = await streamable.Receive(buffer, 0, buffer.Length);
            }
            

            lintener.Close();
            
            NUnit.Framework.Assert.AreEqual(true, breakEvent);
        }
    }
}
