using System;
using System.Linq;

namespace Regulus.Application.Protocol.TextWriter
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Regulus.Utility.Log.Instance.RecordEvent += _WriteLog;
            if (args.Length < 3)
            {
                Console.WriteLine("Need to build parameters.");
                Console.WriteLine("ex. Regulus.Application.Protocol.TextWriter ProtocolName path/in-common.dll /out-path");
                return;
            }
            var protocolName = args[0];
            var inPath = System.IO.Path.GetFullPath(args[1]);
            var outPath = System.IO.Path.GetFullPath(args[2]);
            Console.WriteLine($"Input {inPath}");
            Console.WriteLine($"Output {outPath}");

            var inputAssembly = System.Reflection.Assembly.LoadFile(inPath);

            var builder = new Regulus.Remote.Protocol.CodeBuilder();
            builder.ProviderEvent += (name, code) => _WriteProvider(name, code, outPath);
            builder.EventEvent += (type_name, event_name, code) => _WriteEvent(type_name, event_name, code, outPath);
            builder.GpiEvent += (type_name, code) => _WriteType(type_name, code, outPath);
            builder.Build(protocolName, inputAssembly.GetExportedTypes());
        }

        private static void _WriteType(string type_name, string code, string outPath)
        {
            Regulus.Utility.Log.Instance.WriteInfo($"{outPath}\\{type_name}.cs");

            System.IO.File.WriteAllText($"{outPath}\\{type_name}.cs", code);
        }

        private static void _WriteEvent(string type_name, string event_name, string code, string outPath)
        {
            Regulus.Utility.Log.Instance.WriteInfo($"{outPath}\\{type_name}_{event_name}.cs");

            System.IO.File.WriteAllText($"{outPath}\\{type_name}_{event_name}.cs", code);
        }

        private static void _WriteProvider(string name, string code, string outPath)
        {
            Regulus.Utility.Log.Instance.WriteInfo($"Name:{name}.cs");

            Regulus.Utility.Log.Instance.WriteInfo($"{outPath}\\{name}.cs");
            System.IO.File.WriteAllText($"{outPath}\\{name}.cs", code);
        }

        private static void _WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
