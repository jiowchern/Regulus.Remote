using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNativeUserConsole
{
    using Regulus.Extension;
    public class Program
    {
        static void Main(string[] args)
        {
            Regulus.Utility.Console.IViewer viwer = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(viwer);
            TestNativeUser.Application appliaction = new TestNativeUser.Application(viwer, input);

            appliaction.UserSpawnEvent += (user) => 
            {
                user.MessagerProvider.Supply += (messager) => { _Messager(messager, appliaction.Command, viwer); };
            };
            Regulus.Utility.Updater updater = new Regulus.Utility.Updater();
            appliaction.SetLogMessage(Regulus.Utility.Console.LogFilter.All);
            
            updater.Add(appliaction);
            
            

            bool exit = false;

            appliaction.Command.Register("quit", () => { exit = true; });

            while (exit == false)
            {
                input.Update();
                updater.Update();
            }
            appliaction.Command.Unregister("quit");
        }

        private static void _Messager(TestNativeGameCore.IMessager messager, Regulus.Utility.Command command , Regulus.Utility.Console.IViewer viewer)
        {
            command.RemotingRegister<string,string>("SendMessage", messager.Send, (result) => { viewer.WriteLine(result); });

        }

        

        
    }
   
}
