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
            peer.SocketErrorEvent += (e) => { NUnit.Framework.Assert.Pass(); };
#if DEBUG
            // disconnect test
            await connector.Disconnect(true);


            // reconnect test
            peer = await connector.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port));

            NUnit.Framework.Assert.IsNotNull(peer);
#else
            // disconnect test
            await connector.Disconnect(false);
#endif


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

    }
}
