using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 
namespace AsyncEchoServer
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