// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Regulus.Remoting.Soul.Native;
using Regulus.Utility.WindowConsoleAppliction;

namespace Server
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var app = new Application();
			app.Run();
		}
	}
}