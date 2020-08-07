using Regulus.Remote;
using Regulus.Utility;
using Regulus.Utility.WindowConsoleAppliction;
using System.IO;
using System.Reflection;

namespace Regulus.Application.Client
{
    class Program
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protocol">A protocol path , build from Regulus.Remote.Protocol </param>
        /// <param name="mode">A protocol path , build from Regulus.Remote.Protocol </param>
        /// <param name="entry">A entry path , your game entry dll </param>
        static void Main(System.IO.FileInfo protocol, SOCKETMODE mode = SOCKETMODE.TCP, System.IO.FileInfo entry = null)
        {

            CommandLineHandler cmdHandler = new CommandLineHandler(protocol, entry, mode);
            cmdHandler.RunTcpEvent += _RunTcp;
            cmdHandler.RunWebEvent += _RunWeb;
            cmdHandler.RunStandaloneEvent += _RunStandalone;
            cmdHandler.Process();
        }

        private static void _RunStandalone(FileInfo protocol_path, FileInfo standalone_path)
        {
            IProtocol protocol = _LoadProtocol(protocol_path);
            Regulus.Remote.IEntry entry = _LoadEntry(standalone_path);
            Remote.Client.IAgentProvider agentProvider = null;//todo new Regulus.Remote.Client.AgentProvider(protocol, (p)=> Regulus.Remote.Client.AgentProvider.CreateStandalone(p, entry) );

            ConsoleViewer view = new Regulus.Utility.ConsoleViewer();
            ConsoleInput input = new Regulus.Utility.ConsoleInput(view);
            input.Launch();
            entry.Launch();
            Remote.Client.Console console = new Regulus.Remote.Client.Console(protocol.GetInterfaceProvider().Types, agentProvider, view, input);
            console.CreateUser();
            console.Run(() =>
            {
                input.Update();
            });
            entry.Shutdown();
            input.Shutdown();

        }

        private static IEntry _LoadEntry(FileInfo standalone_path)
        {

            System.Console.WriteLine($"Standalone path = {standalone_path.FullName}.");

            return Regulus.Remote.Loader.Create(System.Reflection.Assembly.LoadFrom(standalone_path.FullName));
        }

        static void _RunTcp(FileInfo protocol_path)
        {
            IProtocol protocol = _LoadProtocol(protocol_path);
            Remote.Client.IAgentProvider agentProvider = null;//todo  new Regulus.Remote.Client.AgentProvider(protocol, Regulus.Remote.Client.AgentProvider.CreateTcp);

            ConsoleViewer view = new Regulus.Utility.ConsoleViewer();
            ConsoleInput input = new Regulus.Utility.ConsoleInput(view);
            input.Launch();
            Remote.Client.Console console = new Regulus.Remote.Client.Console(protocol.GetInterfaceProvider().Types, agentProvider, view, input);
            console.CreateUser();

            console.Run(() => { input.Update(); });
            input.Shutdown();
        }

        static void _RunWeb(FileInfo protocol_path)
        {
            IProtocol protocol = _LoadProtocol(protocol_path);
            Remote.Client.IAgentProvider agentProvider = null;//todo  new Regulus.Remote.Client.AgentProvider(protocol, Regulus.Remote.Client.AgentProvider.CreateWeb);

            ConsoleViewer view = new Regulus.Utility.ConsoleViewer();
            ConsoleInput input = new Regulus.Utility.ConsoleInput(view);
            input.Launch();
            Remote.Client.Console console = new Regulus.Remote.Client.Console(protocol.GetInterfaceProvider().Types, agentProvider, view, input);
            console.CreateUser();

            console.Run(() => { input.Update(); });
            input.Shutdown();
        }

        private static IProtocol _LoadProtocol(FileInfo protocol_path)
        {
            System.Console.WriteLine($"Protocol path = {protocol_path.FullName}.");
            Assembly assembly = System.Reflection.Assembly.LoadFrom(protocol_path.FullName);
            System.Console.WriteLine($"WorkDir = {System.IO.Directory.GetCurrentDirectory()}.");

            IProtocol protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(assembly);
            return protocol;
        }



    }
}
