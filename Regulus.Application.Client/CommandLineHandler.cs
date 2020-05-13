using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace Regulus.Application.Client
{
    class CommandLineHandler
    {
        private readonly FileInfo _Protocol;
        private readonly FileInfo _Standalone;
        public event System.Action<FileInfo> RunTcpEvent;
        public event System.Action<FileInfo, FileInfo> RunStandaloneEvent;
        
        
        public CommandLineHandler(FileInfo protocol, FileInfo standalone)
        {
            this._Protocol = protocol;
            _Standalone = standalone;
            new Option("--protocol").AddAlias("-p");            
            new Option("--entry").AddAlias("-e");

        }

        internal void Process()
        {
            if(_Standalone == null)
                RunTcpEvent(_Protocol);
            else
                RunStandaloneEvent(_Protocol, _Standalone);
        }
    }
}
