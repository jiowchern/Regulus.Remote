using System;
using System.Linq;

namespace Regulus.Application.Protocol.TextWriter
{
    class Program
    {
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="common"></param>
        /// <param name="output"></param>
        static void Main(System.IO.FileInfo common, System.IO.DirectoryInfo output)
        {
            Regulus.Utility.Log.Instance.RecordEvent += _WriteLog;

            var protocolName = $"C{System.Guid.NewGuid().ToString("N")}";
            Regulus.Utility.Log.Instance.WriteInfo($"Protocol name is {protocolName}.");
            var inPath = common.FullName;
            var outPath = output.FullName;
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
