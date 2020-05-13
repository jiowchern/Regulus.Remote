using Regulus.Remote.Unity;
using System;
using System.IO;
using System.Reflection;

namespace Regulus.Application.Protocol.UnityCodeWriter
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="common"></param>
        /// <param name="agent"></param>
        /// <param name="output"></param>
        static void Main(FileInfo common,string agent,DirectoryInfo output )
        {
            var outputer = new CodeOutputer(Assembly.LoadFrom(common.FullName), agent);
            outputer.Output(output.FullName);
        }
    }
}
