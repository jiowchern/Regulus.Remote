using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNativeSoul
{
    class Program
    {
        static void Main(string[] args)
        {
            Regulus.Utility.Console.IViewer viwer = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(viwer);
            var server = new Regulus.Remoting.Soul.Native.Application(viwer, input);

            Regulus.Utility.Updater<Regulus.Utility.IUpdatable> updater = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();
            updater.Add(server);

            bool exit = false;

            server.Command.Register("quit", () => { exit = true; });

            while (exit == false)
            {
                updater.Update();
                input.Update();
            }

            server.Command.Unregister("quit");
        }
    }
}
