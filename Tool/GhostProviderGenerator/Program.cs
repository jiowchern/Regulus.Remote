using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


using Regulus.Utility;

using Console = System.Console;

namespace Regulus.Remote.Protocol.Generator
{
    
    class Program
    {


        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, a) =>
            {
                Regulus.Utility.CrashDump.Write();
                Environment.Exit(0);
            };

            Regulus.Utility.Log.Instance.RecordEvent += _WriteLog;
            if (args.Length == 0)
            {
                Console.WriteLine("Need to build parameters.");
                Console.WriteLine("ex . RegulusProotoclBuilder.exe build.ini");
                return;
            }
            var path = args[0];
            if (System.IO.File.Exists(path) == false)
            {
                Console.WriteLine($"Non-existent path {path}.");
                return;
            }

            try
            {
                var ini = new Regulus.Utility.Ini(System.IO.File.ReadAllText(path));

                var sourcePath = String.Empty;

                if (_TryRead(ini , "Build", "SourcePath", out sourcePath ) == false)
                {
                    Program._ShowBuildIni();
                    return;
                }


                var protocolName = String.Empty;

                if (_TryRead(ini, "Build", "ProtocolName", out protocolName) == false)
                {
                    Program._ShowBuildIni();
                    return;
                }


                var outputPath = String.Empty;
                var outputCode = String.Empty;
                if (_TryRead(ini, "Build", "OutputPath", out outputPath) )
                {
                    var sourceFullPath = System.IO.Path.GetFullPath(sourcePath);
                    var outputFullPath = System.IO.Path.GetFullPath(outputPath);

                    Console.WriteLine($"Source {sourceFullPath}");
                    Console.WriteLine($"Output {outputFullPath}");
                    var sourceAsm = Assembly.LoadFile(sourceFullPath);
                    var assemblyBuilder = new Regulus.Remote.Protocol.AssemblyBuilder();
                    assemblyBuilder.Build(sourceAsm, protocolName, outputFullPath);                    
                    Console.WriteLine("Build success.");

                    
                }
                if (_TryRead(ini, "Build", "OutputCode", out outputCode) )
                {
                    var sourceFullPath = System.IO.Path.GetFullPath(sourcePath);
                    var outputFullPath = System.IO.Path.GetFullPath(outputCode);

                    Console.WriteLine($"Source {sourceFullPath}");
                    Console.WriteLine($"Output {outputFullPath}");
                    var sourceAsm = Assembly.LoadFile(sourceFullPath);
                    var assemblyBuilder = new Regulus.Remote.Protocol.AssemblyBuilder();                    
                    //assemblyBuilder.BuildCode(sourceAsm, protocolName, outputFullPath);
                    throw new NotImplementedException();
                    Console.WriteLine("Build success.");
                    
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Build failure.");                
            }
            
        }

        private static bool _TryRead(Ini ini, string section, string key, out string value)
        {
            if (ini.TryRead(section, key, out value) == false)
            {
                
                return false;
                
            }
            return true;
        }

        private static void _ShowBuildIni()
        {
            Console.WriteLine("Wrong Ini format.");
            var iniSample = @"
[Build]
SourcePath = YourProjectPath/YourAssemblyCommon.dll
ProtocolName = YourProjectnamesapce.ProtocolClassName
OutputPath = YourProjectPath/YourAssemblyOutput.dll
OutputCode = YourProjectPath/CodeDir
";
            Console.WriteLine("ex.");
            Console.WriteLine(iniSample);

            Console.WriteLine("Do you create sample.ini file? (Y/N)");
            var ans = Console.ReadLine();
            if (ans == "Y" || ans == "y")
            {
                System.IO.File.WriteAllText("sample.ini" , iniSample);
            }
        }

        private static void _WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
