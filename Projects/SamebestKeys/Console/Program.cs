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

            var application = new Regulus.Project.SamebestKeys.Console(view, input);
            application.SetLogMessage(Regulus.Utility.Console.LogFilter.All);

            Regulus.Utility.Updater<Regulus.Utility.IUpdatable> updater = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();
            updater.Add(application);
            updater.Add(input);

            bool exit = false;
            application.Command.Register("quit", () => { exit = true; });
            _Initial(application);
    
            while (exit == false)
            {
                updater.Update();                
            }

            application.Command.Unregister("quit");

            
        }

        private static void _Initial(Regulus.Project.SamebestKeys.Console application)
        {
            application.SelectSystemEvent += (selector) =>
            {
                _Spawn(selector.Use("remoting"));
            };
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
