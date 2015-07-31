// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IniTest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IniTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.Utility;

#endregion

namespace PureLibraryTest
{
	[TestClass]
	public class IniTest
	{
		[TestMethod]
		public void TestIni()
		{
			var data = @"[WindowSettings]
Window X Pos=0
Window Y Pos=0
Window Maximized=false
Window Name = Jabberwocky
[Logging]
Directory=C:\Rosetta Stone\Logs
 
[Logging2]
Directory =
";
			var ini = new Ini(data);

			var name = ini.Read("WindowSettings", "Window Name");
			Assert.AreEqual("Jabberwocky", name);

			var max = ini.Read("WindowSettings", "Window Maximized");
			Assert.AreEqual("false", max);

			var dir = ini.Read("Logging", "Directory");
			Assert.AreEqual(@"C:\Rosetta Stone\Logs", dir);

			var dir2 = ini.Read("Logging2", "Directory");
			Assert.AreEqual(string.Empty, dir2);
		}
	}
}