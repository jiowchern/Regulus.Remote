using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Formula;


using Console = System.Console;
using SpinWait = System.Threading.SpinWait;

namespace FormulaUserBot
{
	internal class Program
	{
		private static string IPAddress = "210.65.10.160";

		// private static string IPAddress = "127.0.0.1";
		private static readonly int Port = 38971;

		private static void Main(string[] args)
		{
			var sw = new SpinWait();

			var botCount = 1;
			if(args.Length > 0)
			{
				botCount = int.Parse(args[0]);
			}

			if(args.Length > 1)
			{
				Program.IPAddress = args[1];
			}

			var clientHandler = new ClientHandler(Program.IPAddress, Program.Port, botCount);
			var view = new ConsoleViewer();
			Singleton<Log>.Instance.Initial(view);
			var input = new ConsoleInput(view);
			var client = new Client(view, input);
			var packetRegulator = new PacketRegulator();
			client.Command.Register(
				"si", 
				() =>
				{
					Console.WriteLine(
						"Send Interval : {0}\nRequest Package Queue : {1}", 
						HitHandler.Interval, 
						packetRegulator.Sampling);
				});
			client.ModeSelectorEvent += clientHandler.Begin;

			var updater = new Updater();
			updater.Add(client);
			updater.Add(clientHandler);
			updater.Add(packetRegulator);

			while(client.Enable)
			{
				input.Update();
				updater.Working();
				sw.SpinOnce();
			}

			client.Command.Unregister("si");
			updater.Shutdown();
			clientHandler.End();
			Singleton<Log>.Instance.Final();
		}

		private static void _OnSelector(GameModeSelector<IUser> selector)
		{
			selector.AddFactoty("remoting", new RemotingUserFactory());
			Program._OnProvider(selector.CreateUserProvider("remoting"));
		}

		private static void _OnProvider(UserProvider<IUser> userProvider)
		{
			Program._OnUser(userProvider.Spawn("this"));
			userProvider.Select("this");
		}

		private static void _OnUser(IUser user)
		{
			user.Remoting.ConnectProvider.Supply += Program._Connect;
		}

		private static void _Connect(IConnect obj)
		{
			obj.Connect(Program.IPAddress, Program.Port);
		}
	}
}
