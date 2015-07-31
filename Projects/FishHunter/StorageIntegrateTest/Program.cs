// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Storage;

using Console = System.Console;
using SpinWait = System.Threading.SpinWait;

#endregion

namespace StorageIntegrateTest
{
	internal class Program
	{
		private static bool _Enable;

		private static void Main(string[] args)
		{
			var sw = new SpinWait();
			Program._Enable = true;
			var updater = new Updater();
			var launcher = new Launcher();

			var server = new Server();
			var serverAppliction = new Regulus.Remoting.Soul.Native.Server(server, 12345);

			var client = new Proxy();
			Program.client_UserEvent(client.SpawnUser("user"));

			launcher.Push(serverAppliction);
			updater.Add(client);

			launcher.Launch();
			while (Program._Enable)
			{
				updater.Working();
				sw.SpinOnce();
			}

			updater.Shutdown();
			launcher.Shutdown();

			Console.ReadKey();
		}

		private static void client_UserEvent(IUser usee)
		{
			usee.Remoting.ConnectProvider.Supply += Program.ConnectProvider_Supply;
			usee.VerifyProvider.Supply += Program.VerifyProvider_Supply;
		}

		private static void VerifyProvider_Supply(IVerify obj)
		{
			var result = obj.Login("vgameadmini", string.Empty);
			result.OnValue += val =>
			{
				if (val)
				{
					Console.WriteLine("驗證成功");
				}
				else
				{
					Console.WriteLine("驗證失敗");
				}

				Program._Enable = false;
			};
		}

		private static void ConnectProvider_Supply(IConnect obj)
		{
			var result = obj.Connect("127.0.0.1", 12345);
			result.OnValue += val =>
			{
				if (val)
				{
					Console.WriteLine("連線成功");
				}
				else
				{
					Console.WriteLine("連線失敗");
					Program._Enable = false;
				}
			};
		}
	}
}