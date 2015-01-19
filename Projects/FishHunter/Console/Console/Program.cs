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

            var client = new Regulus.Framework.Client<VGame.Project.FishHunter.IUser>(view, input );
            client.ModeSelectorEvent += client_ModeSelectorEvent;
            var updater = new Regulus.Utility.Updater();
            updater.Add(client);
            while (client.Enable)
            {
                input.Update();
                updater.Update();
            }

            updater.Shutdown();
        }

        static void client_ModeSelectorEvent(Regulus.Framework.GameModeSelector<VGame.Project.FishHunter.IUser> selector)
        {
            selector.AddFactoty( "standalong" , new VGame.Project.FishHunter.StandalongUserFactory(null));
            var provider = selector.CreateGameConsole("standalong");
            provider.Spawn("1"); 
            provider.Select("1");

            provider.Spawn("2");
            provider.Select("2");

            provider.Spawn("3");
            provider.Select("3");

            provider.Spawn("4");
            provider.Select("4");
           
        }
    }
}
