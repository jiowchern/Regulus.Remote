using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Tool
{
    using Regulus.Utility.WindowConsoleAppliction;
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, a) =>
            {
                Regulus.Utility.CrashDump.Write();
                Environment.Exit(0);
            };
            var ini = new Regulus.Utility.Ini(System.IO.File.ReadAllText(args[0]));

            var sourcePath = ini.Read("Setting", "SourcePath");

            var sourceFullPath = System.IO.Path.GetFullPath(sourcePath);
            var sourceNamespace = ini.Read("Setting", "Namespace");
            var protocolName = ini.Read("Setting", "ProtocolName");
            
            var outputPath = ini.Read("Setting", "OutputPath");
            var dumpCode = ini.Read("Setting", "DumpCode");

            var sourceAsm = Assembly.LoadFile(sourceFullPath);
            Regulus.Protocol.AssemblyBuilder ghostProviderGenerator  = new Regulus.Protocol.AssemblyBuilder();
            var codes = ghostProviderGenerator.Build(sourceAsm, outputPath , new [] {sourceNamespace});

            if (dumpCode == "true")
            {
                //System.IO.File.WriteAllLines(outputPath + ".cs" , codes);
            }
        }
    }
}
