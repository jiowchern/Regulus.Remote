using Regulus.Remote;
using Regulus.Utility;
using Regulus.Utility.WindowConsoleAppliction;
using System;
using System.CommandLine;
using System.Reflection;

namespace Regulus.Application.Client
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="protocol">A protocol path , build from Regulus.Remote.Protocol </param>
        static void Main(string protocol)
        {
            var protocolPath = System.IO.Path.GetFullPath(protocol);
            if (System.IO.Path.IsPathRooted(protocol) && System.IO.Path.IsPathFullyQualified(protocol))
                protocolPath = protocol;
            var workDir = System.IO.Path.GetDirectoryName(protocolPath);

            System.Console.WriteLine($"Protocol path is {protocolPath}.");
            
            //System.IO.Directory.SetCurrentDirectory(workDir);            
            var assembly = System.Reflection.Assembly.LoadFrom(protocolPath);
            System.Console.WriteLine($"Work Dir is {System.IO.Directory.GetCurrentDirectory()}.");
            


            _Run(assembly);
        }

        private static void _Run(Assembly assembly)
        {
            var view = new Regulus.Utility.ConsoleViewer();
            
            IProtocol protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(assembly);
            var agentProvider = new Regulus.Remote.Client.AgentProvider(protocol, Regulus.Remote.Client.AgentProvider.CreateTcp);
            var console = new Regulus.Remote.Client.Console(protocol.GetInterfaceProvider().Types, agentProvider, view, new Regulus.Utility.ConsoleInput(view));

            console.Run();
            
        }
    }
}
