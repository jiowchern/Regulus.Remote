using System.Diagnostics;
using System.Net.Mime;


using Regulus.Utility.WindowConsoleAppliction;

namespace Regulus.Project.RestartProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                var app = new Application(args[0]);
                app.Run();
            }
            else
            {
                var app = new Application();
                app.Run();
            }
            
        }
    }
}
