using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

using Console = Regulus.Utility.Console;

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

            var view = new Regulus.Utility.ConsoleViewer() as Console.IViewer;
            
            var logFile = new Regulus.Utility.LogFileRecorder("GhostProviderGenerator");
            Regulus.Utility.Log.Instance.RecordEvent += logFile.Record;
            Regulus.Utility.Log.Instance.RecordEvent += view.WriteLine;

            var ini = new Regulus.Utility.Ini(System.IO.File.ReadAllText(args[0]));

            var sourcePath = ini.Read("Setting", "SourcePath");
            var sourceNamespace = ini.Read("Setting", "Namespace");
            var providerName = ini.Read("Setting", "ProviderName");
            var outputPath = ini.Read("Setting", "OutputPath");
            var dumpCode = ini.Read("Setting", "DumpCode");

            GhostProviderGenerator ghostProviderGenerator  = new GhostProviderGenerator();
            var output = ghostProviderGenerator.BuildProvider(sourcePath, outputPath, providerName, new [] { sourceNamespace });

            if (dumpCode == "true")
            {
                System.IO.File.WriteAllLines(outputPath + ".cs" , output.Codes);                    
            }

            foreach (CompilerError error in output.Result.Errors)
            {                
                Regulus.Utility.Log.Instance.WriteInfo(string.Format("Error : ({0},{1}) {2} {3} : {4}", error.Line , error.Column , error.ErrorNumber , error.ErrorText , error.FileName));
            }
            


        }
    }
}
