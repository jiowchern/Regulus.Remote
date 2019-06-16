using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Application.Protocol.Unity
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Regulus.Utility.Log.Instance.RecordEvent += _WriteLog;
            
            if (args.Length != 1)
            {
                Console.WriteLine("Param 1 Need to build file path.");

                var iniSample = @"
[Build]
common_path = path/common.dll
unityengine_path = path/UnityEngine.dll
agent_name = Namespace.Agent
output_path = path/protocol.dll
output_dir = path/dir
";
                Console.WriteLine("The format is as follows");                
                Console.WriteLine(iniSample);
                System.IO.File.WriteAllText("BuildProtocol.ini" , iniSample);
                return;
            }

            var path = args[0];

            var iniStream = System.IO.File.ReadAllText(path);
            var ini = new Regulus.Utility.Ini(iniStream);

            var commonPath = ini.Read("Build", "common_path");
            var unityenginePath = ini.Read("Build", "unityengine_path");            
            
            var commonNamespace = ini.Read("Build", "agent_name");
            

            
            
            var commandFullPath = System.IO.Path.GetFullPath(commonPath);

            var unityengineFullPath = System.IO.Path.GetFullPath(unityenginePath);

            Console.WriteLine("Common path {0}", commandFullPath);
            Console.WriteLine("UnityEngine path {0}", unityengineFullPath);
            var commonAsm = Assembly.LoadFile(commandFullPath);
            var unityengineAsm = Assembly.LoadFile(unityengineFullPath);


            var regulusLibrary = _GetRegulusLibrary();
            var regulusRemoting = _GetRegulusRemoting();
            var regulusProrocol = _GetRegulusProtocol();
            var regulusProtocolUnity = _GetRegulusProtocolUnity();
            var regulusRemotingGhost = _GetRegulusRemotingGhost();
            var regulusSerialization = _GetRegulusSerialization();

            /*var assemblyOutputer = new AssemblyOutputer(commonAsm,  commonNamespace);
            assemblyOutputer.ErrorMessageEvent += Console.WriteLine;

            string outputPath;
            if (_TryGetIniString(ini, "Build" , "output_path", out outputPath))
            {
                assemblyOutputer.OutputDll(outputPath, unityengineAsm, regulusLibrary, regulusRemoting, regulusProrocol, regulusProtocolUnity, regulusRemotingGhost, regulusSerialization);
            }


            string outputDir;
            if (_TryGetIniString(ini, "Build", "output_dir", out outputDir))
            {
                assemblyOutputer.OutputDir(outputDir);
            }*/




            Regulus.Utility.Log.Instance.Shutdown();

            Console.WriteLine("done");

        }

       

        private static void _WriteLog(string message)
        {
            Console.WriteLine(message);
        }

        private static Assembly _GetRegulusSerialization()
        {
            return _GetAssembly("regulus.Serialization.dll");
        }

        private static Assembly _GetRegulusRemotingGhost()
        {
            return _GetAssembly("Regulus.Remote.Ghost.dll");
        }

        private static Assembly _GetRegulusProtocolUnity()
        {
            return _GetAssembly("Regulus.Remote.Unity.dll");
        }

        private static Assembly _GetRegulusProtocol()
        {
            return _GetAssembly("Regulus.Remote.Protocol.dll");
        }

        private static Assembly _GetRegulusLibrary()
        {
            return _GetAssembly("regulus.library.dll");
        }

        private static Assembly _GetRegulusRemoting()
        {
            return _GetAssembly("regulus.remote.dll");
        }

        private static Assembly _GetAssembly(string filename)
        {
            return Assembly.LoadFile(System.IO.Path.GetFullPath(filename));
        }

        
    }
}


