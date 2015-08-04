// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.ServiceProcess;

#endregion

namespace VGame.Project.FishHunter
{
	internal static class Program
	{
		/// <summary>
		///     應用程式的主要進入點。
		/// </summary>
		private static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new FormulaService()
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}