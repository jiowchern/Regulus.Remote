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
                Console.WriteLine("ex. Regulus.Application.Protocol.CodeWriter ProtocolName path/in-common.dll /out-path");
                return;
            }
            var protocolName = args[0];
            var inPath = System.IO.Path.GetFullPath(args[1]);
            var outPath = System.IO.Path.GetFullPath(args[2]);
            Console.WriteLine($"Input {inPath}");
            Console.WriteLine($"Output {outPath}");

            var inputAssembly = System.Reflection.Assembly.LoadFile(inPath);
            var outputer = new Regulus.Remote.Protocol.CodeOutputer(inputAssembly , protocolName);
            outputer.Output(outPath);
        }

      
        private static void _WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
