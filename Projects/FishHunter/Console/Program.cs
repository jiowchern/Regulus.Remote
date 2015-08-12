// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.IO;

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter;

#endregion

namespace Console
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var view = new ConsoleViewer();
			var input = new ConsoleInput(view);

			var core = Program._LoadGame("Game.dll");

			var client = new Client<IUser>(view, input);
			client.ModeSelectorEvent += new ModeCreator(core).OnSelect;

			var updater = new Updater();
			updater.Add(client);
			updater.Add(core);

			while (client.Enable)
			{
				input.Update();
				updater.Working();
			}

			updater.Shutdown();
		}

		private static ICore _LoadGame(string path)
		{
			var stream = File.ReadAllBytes(path);
			return Loader.Load(stream, "VGame.Project.FishHunter.Play.DummyStandalone");
		}
	}
}