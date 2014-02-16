using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
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

            server.Command.Register("quit", 
                () => 
                { 
                    exit = true; 
                });
            _Initial(server);
            
            while (exit == false)
            {
                updater.Update();
                input.Update();                
            }
            
            server.Command.Unregister("quit");
            updater.Shutdown();
        }


        private static void _Initial(Regulus.Remoting.Soul.Native.Application application)
        {
            application.SelectSystemEvent += (selector) =>
            {
                _Spawn(selector.Use("stand"));
            };
        }

        private static void _Spawn(Regulus.Remoting.Value<Regulus.Game.ConsoleFramework<Regulus.Remoting.Soul.Native.IUser>.IUserRequester> value)
        {
            value.OnValue += _SpawnController;
        }

        static void _SpawnController(Regulus.Game.ConsoleFramework<Regulus.Remoting.Soul.Native.IUser>.IUserRequester obj)
        {
            obj.Spawn("jc", true);
        }
    }
}
