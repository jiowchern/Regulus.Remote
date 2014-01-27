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

            var server = new Regulus.Remoting.Soul.NativeAppliaction(12345, viwer, input);

            var client = new Regulus.Remoting.Ghost.Native.Agent("127.0.0.1" , 12345);

            Regulus.Game.IFramework servera = server;

            Regulus.Utility.Updater<Regulus.Utility.IUpdatable> updater = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();
            updater.Add(server);
            updater.Add(client);
            servera.Launch();

            server.Command.Register("ping", () => 
            {

                viwer.WriteLine(System.TimeSpan.FromTicks(client.Ping).ToString());
            });
            while(true)
            {
                servera.Update();
                updater.Update();
                input.Update();
                
            }
            servera.Shutdown();
            
            
        }
    }
}