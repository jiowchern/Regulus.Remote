


using Regulus.Utility;

namespace RegulusLibraryTest
{
	
	public class IniTest
	{
		[NUnit.Framework.Test()]
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
			NUnit.Framework.Assert.AreEqual("Jabberwocky", name);

			var max = ini.Read("WindowSettings", "Window Maximized");
			NUnit.Framework.Assert.AreEqual("false", max);

			var dir = ini.Read("Logging", "Directory");
			NUnit.Framework.Assert.AreEqual(@"C:\Rosetta Stone\Logs", dir);

			var dir2 = ini.Read("Logging2", "Directory");
			NUnit.Framework.Assert.IsTrue(string.IsNullOrWhiteSpace(dir2));
		}
	}
}
