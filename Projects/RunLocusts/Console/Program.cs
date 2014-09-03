using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;
            Regulus.Utility.ConsoleViewer view = new Regulus.Utility.ConsoleViewer();
            Regulus.Utility.ConsoleInput input = new Regulus.Utility.ConsoleInput(view);

            Imdgame.RunLocusts.Client client = new Imdgame.RunLocusts.Client(view, input);

            Regulus.Utility.Updater updater = new Regulus.Utility.Updater();

            client.Command.Register("quit", () => { exit = true; });

            updater.Add(client);
            updater.Add(input);

            
            while (exit == false)
            {
                updater.Update();                
            }

            updater.Shutdown();
        }
    }
}
