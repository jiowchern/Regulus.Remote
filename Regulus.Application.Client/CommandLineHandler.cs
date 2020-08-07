using System.CommandLine;
using System.IO;

namespace Regulus.Application.Client
{
    class CommandLineHandler
    {
        private readonly FileInfo _Protocol;
        private readonly FileInfo _Standalone;
        private readonly SOCKETMODE mode;

        public event System.Action<FileInfo> RunTcpEvent;
        public event System.Action<FileInfo> RunWebEvent;
        public event System.Action<FileInfo, FileInfo> RunStandaloneEvent;


        public CommandLineHandler(FileInfo protocol, FileInfo standalone, SOCKETMODE mode)
        {
            this._Protocol = protocol;
            _Standalone = standalone;
            this.mode = mode;
            new Option("--protocol").AddAlias("-p");
            new Option("--entry").AddAlias("-e");

        }

        internal void Process()
        {
            if (_Standalone == null)
                if (mode == SOCKETMODE.TCP)
                    RunTcpEvent(_Protocol);
                else
                    RunWebEvent(_Protocol);

            else
                RunStandaloneEvent(_Protocol, _Standalone);
        }
    }
}
