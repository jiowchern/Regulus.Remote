using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var view = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(view);
            
            var application = new Regulus.Project.SamebestKeys.Console(view, input);

            _BuildGameData(view);
            BotSet bots = new BotSet();


            application.SetLogMessage(Regulus.Utility.Console.LogFilter.All);

            Regulus.Utility.Updater updater = new Regulus.Utility.Updater();
            
            updater.Add(application);
            updater.Add(input);
            updater.Add(bots);

            bool exit = false;
            application.Command.Register("quit", () => { exit = true; });
            application.Command.Register<int>("BotSn", (sn) =>
            {
                bots.Sn = sn;
            });
            application.UserRequesterEvent += (requester) =>
            {
                bots.Requester = requester;
            };
            application.Command.Register<int>("Bot", (count) => 
            {
                bots.Create(count );
            });
            /*application.SelectSystemEvent += (selector) =>
            {
                var value = selector.Use("remoting");
                value.OnValue += _SpawnController;
                value.OnValue += (requester) => 
                {
                    bots.Requester = requester;
                };
            };*/

            Regulus.Utility.TimeCounter fps = new Regulus.Utility.TimeCounter(); ;
            while (exit == false)
            {                
                updater.Update();
                fps.Reset();                
            }
            application.Command.Unregister("BotSn");
            application.Command.Unregister("Bot");
            application.Command.Unregister("quit");
            application.Stop();

            
        }

        private static void _BuildGameData(Regulus.Utility.Console.IViewer view)
        {
            var dataBuilder = new Regulus.Project.SamebestKeys.GameDataBuilder(Regulus.Project.SamebestKeys.GameData.Instance);
            var workPath = System.IO.Directory.GetCurrentDirectory();
            var files = System.IO.Directory.GetFiles(workPath, "*.map.txt");
            foreach (var file in files)
            {
                var stream = System.IO.File.ReadAllBytes(file);
                view.WriteLine("load game data : " + file);
                dataBuilder.Build(stream);
            }
        }


        

        private static void _Spawn(Regulus.Remoting.Value<Regulus.Game.ConsoleFramework<Regulus.Project.SamebestKeys.IUser>.IUserRequester> value)
        {
            value.OnValue += _SpawnController;
        }

        static void _SpawnController(Regulus.Game.ConsoleFramework<Regulus.Project.SamebestKeys.IUser>.IUserRequester obj)
        {
            obj.Spawn("jc", true);
        }

        
    }
}
