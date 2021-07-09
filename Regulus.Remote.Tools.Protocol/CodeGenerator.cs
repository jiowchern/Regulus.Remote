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
        public string RemoteFile { get; set; }


        public override bool Execute()
        {            
            var args = new GenerateArguments("./", new TaskItem(InputFile));
            Log.LogMessage(MessageImportance.High, $"RegulusRemoteProtocol Input is {args.InputFullFile}");
            Log.LogMessage(MessageImportance.High, $"RegulusRemoteProtocol Output is {args.OuputFullDir}");
            Log.LogMessage(MessageImportance.High, $"RegulusRemoteProtocol Current is {System.IO.Directory.GetCurrentDirectory()}");
            if (!System.IO.File.Exists(args.InputFullFile))
                return false;
            if (!System.IO.Directory.Exists(args.OuputFullDir))
                return false;
            var asm = System.Reflection.Assembly.LoadFile(args.InputFullFile);
            var outputer = new Regulus.Remote.Protocol.CodeOutputer(asm);
            outputer.Output(args.OuputFullDir);
            return !Log.HasLoggedErrors;
        }
    }
}
