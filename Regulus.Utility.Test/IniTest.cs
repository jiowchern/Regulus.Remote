


using Regulus.Utility;

namespace RegulusLibraryTest
{

    public class IniTest
    {
        [Xunit.Fact]
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
            Ini ini = new Ini(data);

            string name = ini.Read("WindowSettings", "Window Name");
            Xunit.Assert.Equal("Jabberwocky", name);

            string max = ini.Read("WindowSettings", "Window Maximized");
            Xunit.Assert.Equal("false", max);

            string dir = ini.Read("Logging", "Directory");
            Xunit.Assert.Equal(@"C:\Rosetta Stone\Logs", dir);

            string dir2 = ini.Read("Logging2", "Directory");
            Xunit.Assert.True(string.IsNullOrWhiteSpace(dir2));
        }
    }
}
