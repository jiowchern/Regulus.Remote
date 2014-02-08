using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNativeUserConsole
{
    using Regulus.Extension;


    class BatchCommander
    {
        private TestNativeUser.Application _App;
        Regulus.Utility.Console.IViewer _View;
        struct CommandString
        {
            public string Name;
            public string[] Args;
        }
        Regulus.Utility.TimeCounter _Timer;
        Queue<CommandString> _CommandStrings;
        public BatchCommander(TestNativeUser.Application app , Regulus.Utility.Console.IViewer view)
        {
            _Timer = new Regulus.Utility.TimeCounter();

            this._App = app;

            _View = view;
            _App.Command.Register("1", _1);
            _App.Command.Register<int>("2", _2);
            _CommandStrings = new Queue<CommandString>();




        }

        private void _2(int count)
        {
            _CommandStrings.Enqueue(new CommandString() { Name = "remoting", Args = new string[] { } });

            for(int i = 0 ; i < count; ++i)
            {
                _CommandStrings.Enqueue(new CommandString() { Name = "spawncontroller", Args = new string[] { "jc"+i.ToString() } });
                _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "jc" + i.ToString() } });

                _CommandStrings.Enqueue(new CommandString() { Name = "connect", Args = new string[] { "127.0.0.1", "12345" } });
            }
        }



        private void _1()
        {
            _CommandStrings.Enqueue(new CommandString() { Name = "remoting", Args = new string[] { } });

            _CommandStrings.Enqueue(new CommandString() { Name = "spawncontroller", Args = new string[] { "jc" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "jc" } });

            _CommandStrings.Enqueue(new CommandString() { Name = "connect", Args = new string[] { "127.0.0.1" , "12345" } });

            _App.UserSpawnEvent += (user) =>
            {
                user.MessagerProvider.Supply += (messager) => { _Messager(messager, _App.Command, _View); };
            };


        }
        private static void _Messager(TestNativeGameCore.IMessager messager, Regulus.Utility.Command command, Regulus.Utility.Console.IViewer viewer)
        {
            command.RemotingRegister<string, string>("SendMessage", messager.Send, (result) => { viewer.WriteLine(result); });
        }



        internal void Update()
        {
            if (new System.TimeSpan(_Timer.Ticks).TotalSeconds > 0.5)
            {
                _Timer.Reset();
                if (_CommandStrings.Count > 0)
                {
                    var c = _CommandStrings.Dequeue();
                    _App.Command.Run(c.Name, c.Args);
                }
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Regulus.Utility.Console.IViewer viwer = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(viwer);
            TestNativeUser.Application appliaction = new TestNativeUser.Application(viwer, input);

            


            Regulus.Utility.Updater updater = new Regulus.Utility.Updater();
            appliaction.SetLogMessage(Regulus.Utility.Console.LogFilter.All);
            
            updater.Add(appliaction);
            bool exit = false;

            appliaction.Command.Register("quit", () => { exit = true; });


            var betch = new BatchCommander(appliaction, viwer);
            while (exit == false)
            {
                input.Update();
                updater.Update();
                betch.Update();
            }
            appliaction.Command.Unregister("quit");
        }
        

        

        

        
    }
   
}
