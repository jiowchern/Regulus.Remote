using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PureLibTest
{
    [TestClass]
    public class IniTest
    {
        [TestMethod]
        public void TestIni()
        {
            string data = @"[WindowSettings]
Window X Pos=0
Window Y Pos=0
Window Maximized=false
Window Name = Jabberwocky
[Logging]
Directory=C:\Rosetta Stone\Logs
 
[Logging2]
Directory =
";
            var ini = new Regulus.Utility.Ini(data);

            var name = ini.Read("WindowSettings", "Window Name");
            Assert.AreEqual("Jabberwocky", name);

            var max = ini.Read("WindowSettings", "Window Maximized");
            Assert.AreEqual("false", max);
            
            var dir = ini.Read("Logging", "Directory");
            Assert.AreEqual(@"C:\Rosetta Stone\Logs", dir);

            var dir2 = ini.Read("Logging2", "Directory");
            Assert.AreEqual(@"", dir2);

        }
    }
}
