using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace Regulus.Remote.Tools.Protocol
{
    public class CodeGenerator : Microsoft.Build.Utilities.Task
    {  
        [Required]
        public string InputFile { get; set; }
        [Required]
        public string OutputDir { get; set; }


        public override bool Execute()
        {
            var inputPath = System.IO.Path.GetFullPath(InputFile);
            var outputDir = System.IO.Path.GetFullPath(OutputDir); 
            Log.LogMessage(MessageImportance.High, $"RegulusRemoteProtocol Input is {inputPath}");
            Log.LogMessage(MessageImportance.High, $"RegulusRemoteProtocol Output is {outputDir}");
            if (!System.IO.File.Exists(inputPath))
                return false;
            if (!System.IO.File.Exists(outputDir))
                return false;

            /*var asm = System.Reflection.Assembly.LoadFile(inputPath);
            var outputer = new Regulus.Remote.Protocol.CodeOutputer(asm);
            outputer.Output(outputDir); */
            
            return true;
        }
    }
}
