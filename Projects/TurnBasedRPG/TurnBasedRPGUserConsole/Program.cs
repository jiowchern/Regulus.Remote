using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPGUserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Regulus.Project.TurnBasedRPGUserConsole.Application app = new Regulus.Project.TurnBasedRPGUserConsole.Application();
            app.Run();

            Console.WriteLine("按任一鍵關閉視窗...");
            Console.ReadKey();
        }
    }
}
