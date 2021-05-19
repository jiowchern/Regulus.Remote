using System.Reactive.Linq;
using System.Linq;

namespace Regulus.Network.Tests
{
    public class WebSocketConnectTest
    {
        [Xunit.Fact]
        public async System.Threading.Tasks.Task Test()
        {
            
            var listener = new Regulus.Network.Web.Listener();            
            
            listener.Bind("http://127.0.0.1:12345/");
            var peers = new System.Collections.Concurrent.ConcurrentQueue<Web.Peer>();

            listener.AcceptEvent += peers.Enqueue;            

            var connecter = new Regulus.Network.Web.Connecter(new System.Net.WebSockets.ClientWebSocket());
            var connectResult = await connecter.ConnectAsync("ws://127.0.0.1:12345/");
            
            Xunit.Assert.True(connectResult);

            var ar = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());

            Web.Peer peer;
            while (!peers.TryDequeue(out peer))
            {
                ar.Operate();
            }
            IStreamable server = peer;
            var serverReceiveBuffer = new byte[5];
            var serverReceiveTask = server.Receive(serverReceiveBuffer, 0, 5);
            IStreamable client = connecter;
            var clientSendCount = await client.Send(new byte[] {1,2,3,4,5}, 0 , 5);
            
            var serverReceiveCount = await serverReceiveTask;

            Xunit.Assert.Equal(5, serverReceiveCount);
            Xunit.Assert.Equal(5, clientSendCount);
        }
    }
}
