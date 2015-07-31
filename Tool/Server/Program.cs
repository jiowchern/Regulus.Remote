// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting.Soul.Native;
using Regulus.Utility.WindowConsoleAppliction;

#endregion

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