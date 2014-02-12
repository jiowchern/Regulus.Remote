using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.SamebestKeysUserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Regulus.Project.SamebestKeysUserConsole.Application app = new Regulus.Project.SamebestKeysUserConsole.Application();
            app.Run();

            Console.WriteLine("按任一鍵關閉視窗...");
            Console.ReadKey();
        }
    }
}
