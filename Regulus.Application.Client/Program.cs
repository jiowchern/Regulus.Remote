using Regulus.Utility.WindowConsoleAppliction;
using System;

namespace Regulus.Application.Client
{
    class Program
    {
        static void Main(string[] args)
        {

            var view = new Regulus.Utility.ConsoleViewer();
            var console = new Regulus.Remote.Client.Console(view, new Regulus.Utility.ConsoleInput(view));
            console.Run();
        }
    }
}
