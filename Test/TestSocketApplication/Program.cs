using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

using Regulus.Utility.WindowConsoleAppliction; 
namespace AsyncEchoServer
{
    class BatchCommander
    {
        private Regulus.Utility.Command _Command;

        struct CommandString
        {
            public string Name;
            public string[] Args;
        }
        Regulus.Utility.TimeCounter _Timer;
        Queue<CommandString> _CommandStrings;
        public BatchCommander(Regulus.Utility.Command command)
        {
            _Timer = new Regulus.Utility.TimeCounter();

            this._Command = command;


            _Command.Register("1" , _1);
            _CommandStrings = new Queue<CommandString>();




        }

        

        private void _1()
        {
            _CommandStrings.Enqueue(new CommandString() { Name = "stand", Args = new string[] { } });

            _CommandStrings.Enqueue(new CommandString() { Name = "spawncontroller", Args = new string[] { "jc" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "jc" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "launchini", Args = new string[] { "s.ini" } });
            //_CommandStrings.Enqueue(new CommandString() { Name = "ready", Args = new string[] { } });

            //_CommandStrings.Enqueue(new CommandString() { Name = "login", Args = new string[] { "1", "1" } });
            //_CommandStrings.Enqueue(new CommandString() { Name = "go", Args = new string[] { } });
        }




        internal void Update()
        {
            if (new System.TimeSpan(_Timer.Ticks).TotalSeconds > 0.5)
            {
                _Timer.Reset();
                if (_CommandStrings.Count > 0)
                {
                    var c = _CommandStrings.Dequeue();
                    _Command.Run(c.Name, c.Args);
                }
            }
        }
    }

    
    class Program
    {
        static void Main(string[] args)
        {
            
            var server = new Regulus.Remoting.Soul.Native.Application();
            
            server.Run();
        }
    }
}