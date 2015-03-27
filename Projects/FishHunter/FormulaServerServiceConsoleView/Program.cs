using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaServerServiceConsoleView
{
    class Program
    {
        static void Main(string[] args)
        {
            

            bool enable = true;
            var view = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(view);

            Regulus.Utility.Console console = new Regulus.Utility.Console(input, view);
            console.Command.Register("quit" ,()=> enable = false);
            console.Command.Register("CoreFPS", _CoreFPS );
     
            while(enable)
            {
                input.Update();
            }

        }

        async private static void _CoreFPS()
        {
            FormulaServiceReference.FormulaServiceClient client = new FormulaServiceReference.FormulaServiceClient("BasicHttpBinding_IFormulaService");
            System.Console.WriteLine(await client.GetCoreFPSAsync());
        }

        
    }
}
