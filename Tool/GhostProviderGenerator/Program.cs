using System;
using System.Collections.Generic;
using System.Linq;

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
            var sourceNamespace = ini.Read("Setting", "Namespace");
            var providerName = ini.Read("Setting", "ProviderName");
            var outputPath = ini.Read("Setting", "OutputPath");
            var dumpCode = ini.Read("Setting", "DumpCode");

            GhostProviderGenerator ghostProviderGenerator  = new GhostProviderGenerator();
            var codes = ghostProviderGenerator.Build(sourcePath, outputPath, providerName, new string[] { sourceNamespace });

            if (dumpCode == "true")
            {
                System.IO.File.WriteAllLines(outputPath+".cs" , codes);
            }
        }
    }
}
