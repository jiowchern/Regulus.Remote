using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VGame.Project.FishHunter.Formula;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var view = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(view);
            Regulus.Remoting.ICore core = null;// _LoadGame("Game.dll");

            var client = new VGame.Project.FishHunter.Formula.Client(view, input);

            client.ModeSelectorEvent += new ModeCreator(core).OnSelect;

            var updater = new Regulus.Utility.CenterOfUpdateable();
            updater.Add(client);
           // updater.Add(core);

            while (client.Enable)
            {
                input.Update();
                updater.Working();
            }

            updater.Shutdown();
        }

        private static Regulus.Remoting.ICore _LoadGame(string path)
        {
            var stream = System.IO.File.ReadAllBytes(path);
            return Regulus.Utility.Loader.Load(stream, "VGame.Project.FishHunter.Formula.Center");
        }
    }
}
