using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Regulus.Utility.WindowConsoleAppliction;

namespace Regulus.Network.Tests.TestTool
{
	class Program
	{
		static void Main(string[] args)
		{
		    Application.EnableVisualStyles();
            

			var console = new ToolConsole();
			console.Run();
        }
	}
}
