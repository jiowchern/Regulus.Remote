using Microsoft.Build.Framework;
using System;

namespace Regulus.Remote.Tools.Protocol
{
    public class CodeGenerator : Microsoft.Build.Utilities.Task
    {  
        public string ProtocolFilesOutputDir { get; set; }



        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal , "1 2dddd dddd3");
            return true;
        }


    }
}
