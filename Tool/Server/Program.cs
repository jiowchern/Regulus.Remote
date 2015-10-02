using System;
using System.Collections.Generic;


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
            List<string> command = new List<string>();
            
            foreach (var a in args)
                command.AddRange(a.Split(
		            new[]
		            {
		                ' '
		            },
		            StringSplitOptions.RemoveEmptyEntries));
			var app = new Application(command.ToArray());

			app.Run();
		}
	}
}
