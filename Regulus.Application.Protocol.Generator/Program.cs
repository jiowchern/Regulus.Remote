using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


using Regulus.Utility;

using Console = System.Console;

namespace Regulus.Application.Protocol
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
            if (args.Length < 2)
            {
                Console.WriteLine("Need to build parameters.");
                Console.WriteLine("ex . Regulus.Application.Protocol.Generator.exe path/in-common.dll path/out-protocol.dll");
                return;
            }
            var inPath = args[0];
            var outPath = args[1];


            try
            {


                if (!System.IO.File.Exists(inPath))
                {
                    throw new Exception($"can not found {inPath}");
                }




                var sourceFullPath = System.IO.Path.GetFullPath(inPath);
                var outputFullPath = System.IO.Path.GetFullPath(outPath);

                Console.WriteLine($"Source {sourceFullPath}");
                Console.WriteLine($"Output {outputFullPath}");
                var sourceAsm = Assembly.LoadFile(sourceFullPath);
                var libraryAsm = Assembly.LoadFile(System.IO.Path.GetFullPath("Regulus.Library.dll"));
                var remoteAsm = Assembly.LoadFile(System.IO.Path.GetFullPath("Regulus.Remote.dll"));
                var serizlizationAsm = Assembly.LoadFile(System.IO.Path.GetFullPath("Regulus.Serialization.dll"));
                var assemblyBuilder = new Regulus.Remote.Protocol.AssemblyBuilder(sourceAsm.GetExportedTypes());
                assemblyBuilder.CreateFile(outputFullPath);

                Console.WriteLine("Build success.");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Build failure.");
            }

        }

        private static void _SaveToFile(Assembly assembly, string outputPath)
        {


            byte[] dllAsArray;
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                formatter.Serialize(stream, assembly);

                dllAsArray = stream.ToArray();

            }

            System.IO.File.WriteAllBytes(outputPath, dllAsArray);
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
OutputPath = YourProjectPath/YourAssemblyOutput.dll
";
            Console.WriteLine("ex.");
            Console.WriteLine(iniSample);

            Console.WriteLine("Do you create sample.ini file? (Y/N)");
            var ans = Console.ReadLine();
            if (ans == "Y" || ans == "y")
            {
                System.IO.File.WriteAllText("sample.ini", iniSample);
            }
        }

        private static void _WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
