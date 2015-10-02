using Regulus.Remoting.Soul.Native;
using Regulus.Utility.WindowConsoleAppliction;

using NLog;
using NLog.Fluent_2;

namespace Server
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var app = new Application(args);
			app.Run();
		}
	}
}
