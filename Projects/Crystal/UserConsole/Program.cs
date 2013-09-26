using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserConsole
{
    class Program
    {
        static bool bRun = false;
        public static void Quit()
        {
            bRun = false;
        }
        static void Main(string[] args)
        {
            var console = new Regulus.Utility.Console();

            
            bRun = true;
            var view = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(view);
            console.Initial(input , view);

            console.Command.Register("quit", Quit);
            while (bRun)
            {
                input.Update();
            }
            console.Release();
            console.Command.Unregister("quit");
        }



		
		
    }
}
