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

            Regulus.Utility.Log.Instance.RecordEvent += _WriteLog;
            if (args.Length < 5)
            {
                Console.WriteLine("Need to build parameters.");
                Console.WriteLine("ex. Regulus.Application.Protocol.Generator path/in-common.dll  path/in-RegulusUtility.dll path/in-RegulusRemote.dll path/in-Regulus.Serialization.dll  path/in-netstandard.dll path/in-System.Runtime.dll path/in-System.Linq.Expressions.dll path/in-System.Collections.dll  path/out-protocol.dll");
                return;
            }
            var inPaths = args.Skip(0).Take(args.Length  - 1).Select( a=> System.IO.Path.GetFullPath(a) ).ToArray(); 
            var outPath = System.IO.Path.GetFullPath(args[args.Length-1]);


            try
            {

                foreach(var inPath in inPaths)
                {
                    Console.WriteLine($"Input {inPath}");
                }


                Console.WriteLine($"Output {outPath}");
                var inputAssemblys = inPaths.Select(in_path => Assembly.LoadFile(in_path)) ;
                
                var assemblyBuilder = new Regulus.Remote.Protocol.AssemblyBuilder(Remote.Protocol.Essential.Create(inputAssemblys.ToArray()));
                assemblyBuilder.CreateFile(outPath);

                Console.WriteLine("Build success.");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Build failure.");
            }

        }
        private static void _WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
