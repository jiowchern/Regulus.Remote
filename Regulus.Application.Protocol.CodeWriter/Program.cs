using System;

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
            string inPath = common.FullName;
            string outPath = output.FullName;
            Console.WriteLine($"Input {inPath}");
            Console.WriteLine($"Output {outPath}");

            System.Reflection.Assembly inputAssembly = System.Reflection.Assembly.LoadFile(inPath);
            Remote.Protocol.CodeOutputer outputer = new Regulus.Remote.Protocol.CodeOutputer(inputAssembly);
            outputer.Output(outPath);
        }


        private static void _WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
