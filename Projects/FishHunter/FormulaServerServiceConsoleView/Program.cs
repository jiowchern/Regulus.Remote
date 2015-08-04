// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using FormulaServerServiceConsoleView.Service_References.FormulaServiceReference;

using Regulus.Utility;

#endregion

namespace FormulaServerServiceConsoleView
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var enable = true;
			var view = new ConsoleViewer();
			var input = new ConsoleInput(view);

			var console = new Console(input, view);
			console.Command.Register("quit", () => enable = false);
			console.Command.Register("CoreFPS", Program._CoreFPS);

			while (enable)
			{
				input.Update();
			}
		}

		private static async void _CoreFPS()
		{
			var client = new FormulaServiceClient("BasicHttpBinding_IFormulaService");
			System.Console.WriteLine(await client.GetCoreFPSAsync());
		}
	}
}