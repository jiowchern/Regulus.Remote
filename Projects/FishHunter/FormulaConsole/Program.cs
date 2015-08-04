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

using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var view = new ConsoleViewer();
			var input = new ConsoleInput(view);
			ICore core = null; // _LoadGame("Game.dll");

			var client = new Client(view, input);

			client.ModeSelectorEvent += new ModeCreator(core).OnSelect;

			var updater = new Updater();
			updater.Add(client);

			// updater.Add(core);
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
			return Loader.Load(stream, "VGame.Project.FishHunter.Formula.Center");
		}
	}
}