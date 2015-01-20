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
            var view = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(view);

            

            Regulus.Utility.ICore core = _LoadGame("Game.Dll");

            var client = new Regulus.Framework.Client<VGame.Project.FishHunter.IUser>(view, input);
            client.ModeSelectorEvent += new ModeCreator(core).OnSelect;

            var updater = new Regulus.Utility.Updater();
            updater.Add(client);
            updater.Add(core);
            
            while (client.Enable)
            {
                input.Update();
                updater.Update();
            }

            updater.Shutdown();
        }

        private static Regulus.Utility.ICore _LoadGame(string path)
        {
            var stream = System.IO.File.ReadAllBytes(path);
            return Regulus.Utility.Loader.Load(stream, "VGame.Project.FishHunter.Center");
        }
    }
}
