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

    }
}