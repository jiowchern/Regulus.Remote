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
            System.Collections.Generic.List<Web.Peer> peers = new System.Collections.Generic.List<Web.Peer>(); ;
            listener.AcceptEvent += peers.Add;
            listener.Bind("http://127.0.0.1:12345/");
            


            var connecter = new Regulus.Network.Web.Connecter(new System.Net.WebSockets.ClientWebSocket());
            var connectResult = await connecter.ConnectAsync("ws://127.0.0.1:12345/");

            Xunit.Assert.True(connectResult);

            IStreamable server = peers.Single() ;
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
