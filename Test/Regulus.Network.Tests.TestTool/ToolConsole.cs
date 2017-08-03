using System.Net;
using System.Threading;
using System.Windows.Forms;
using Regulus.Network.Rudp;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.TestTool
{
	internal class ToolConsole : Regulus.Utility.WindowConsole
	{
	    private int _Id;


	    private readonly Regulus.Utility.Updater _Updater;
        public ToolConsole()
		{
		    _Updater = new Updater();

		}

		protected override void _Launch()
		{		    
			Command.RegisterLambda(this , instance => instance.CreateClient() );
		    Command.RegisterLambda(this, instance => instance.CreateServer());
		    Command.RegisterLambda<ToolConsole,string>(this, (instance , ipaddress)=> instance.Watch(ipaddress));

		    Command.RegisterLambda(this, instance => instance.Fake());
        }

	    

	    private void Watch(string ipaddress)
	    {
	        ThreadPool.QueueUserWorkItem(_RunProfile , ipaddress);
	    }

	    private void _RunProfile(object state)
	    {
	        var profile = new PeerProfile((string)state);
            Application.Run(profile);
        }


	    protected override void _Shutdown()
		{
		    Command.Unregister("CreateClient");
		    Command.Unregister("CreateServer");
		    Command.Unregister("Watch");
		    Command.Unregister("Fake");

            _Updater.Shutdown();

        
		}

		protected override void _Update()
		{
		    _Updater.Working();

            
		}

	    public void CreateServer()
	    {
            var server = new RudpServer(new UdpSocket());
	        _Updater.Add(new ServerHandler(++_Id , server, Command , Viewer));
	    }

	    public void CreateClient()
	    {
            var client = new RudpClient(new UdpSocket());
	        _Updater.Add(new ClientHandler(++_Id, client, Command, Viewer));
        }
	    private void Fake()
	    {
	        var clientSocket = new FakeSocket(new IPEndPoint(IPAddress.Parse("0.0.0.1"), 0), Command,this.Viewer);
	        var serverSocket = new FakeSocket(new IPEndPoint(IPAddress.Parse("0.0.0.2"), 0), Command, this.Viewer);

	        clientSocket.SendEvent += serverSocket.Receive;
	        serverSocket.SendEvent += clientSocket.Receive;

            var server = new RudpServer(clientSocket);
	        var client = new RudpClient(serverSocket);

	        _Updater.Add(clientSocket);
	        _Updater.Add(serverSocket);

            _Updater.Add(new ClientHandler(++_Id, client, Command, Viewer));
	        _Updater.Add(new ServerHandler(++_Id, server, Command, Viewer));
        }
    }
}