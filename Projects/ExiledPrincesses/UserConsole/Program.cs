using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserConsole
{
    class BatchCommander
    {
        private Regulus.Utility.Command _Command;

        struct CommandString
        { 
            public string Name ;
            public string[] Args;
        }
        Regulus.Utility.TimeCounter _Timer;
        Queue<CommandString> _CommandStrings;
        public BatchCommander(Regulus.Utility.Command command, Regulus.Utility.ConsoleViewer viewer)
        {
            _Timer = new Regulus.Utility.TimeCounter();

            command.Register("1", _1);
            command.Register("2", _2);
            command.Register("3", _3);
            command.Register("4", _4);
            
            this._Command = command;
            
            

            _CommandStrings = new Queue<CommandString>();
            _CommandStrings.Enqueue(new CommandString() { Name = "standalong" , Args = new string[]{}});
        
            _CommandStrings.Enqueue(new CommandString() { Name = "spawncontroller", Args = new string[] { "茄子"} });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "茄子" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "ready", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "login", Args = new string[] { "1" ,"1" } });        
            _CommandStrings.Enqueue(new CommandString() { Name = "selectactor", Args = new string[] {"123" } });

            _CommandStrings.Enqueue(new CommandString() { Name = "spawncontroller", Args = new string[] { "彥龍" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "彥龍" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "ready", Args = new string[] {} });
            _CommandStrings.Enqueue(new CommandString() { Name = "login", Args = new string[] { "2", "1" } });        
            _CommandStrings.Enqueue(new CommandString() { Name = "selectactor", Args = new string[] { "456" } });


            /*_CommandStrings.Enqueue(new CommandString() { Name = "spawncontroller", Args = new string[] { "茄子1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "茄子1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "login", Args = new string[] { "1", "1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectactor", Args = new string[] { "123" } });

            _CommandStrings.Enqueue(new CommandString() { Name = "spawncontroller", Args = new string[] { "彥龍1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "彥龍1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "login", Args = new string[] { "2", "1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectactor", Args = new string[] { "456" } });*/

            _CommandStrings.Enqueue(new CommandString() { Name = "inbattle", Args = new string[] { } });
            
        
        }

        private void  _1()
        {
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "茄子" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "querypet", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "HandArea", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "EnableZone", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "covercard", Args = new string[] { "0" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "HandArea", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "EnableZone", Args = new string[] { } });

            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "彥龍" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "querypet", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "HandArea", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "EnableZone", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "covercard", Args = new string[] { "0,1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "HandArea", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "EnableZone", Args = new string[] { } });

            /*_CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "茄子1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "querypet", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "HandArea", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "EnableZone", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "covercard", Args = new string[] { "0" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "HandArea", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "EnableZone", Args = new string[] { } });

            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "彥龍1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "querypet", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "HandArea", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "EnableZone", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "covercard", Args = new string[] { "0,1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "HandArea", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "EnableZone", Args = new string[] { } });*/
        }
        private void _2()
        {
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "茄子" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "capture", Args = new string[] { "0" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "QueryEnergys", Args = new string[] { } });


            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "彥龍" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "capture", Args = new string[] { "1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "QueryEnergys", Args = new string[] { } });

            /*_CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "茄子1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "capture", Args = new string[] { "2" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "QueryEnergys", Args = new string[] { } });


            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "彥龍1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "capture", Args = new string[] { "3" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "QueryEnergys", Args = new string[] { } });*/

            
        }
        private void _3()
        {
            
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "茄子" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "QuerySpeed", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "CardLaunched", Args = new string[] { "0" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "Done", Args = new string[] { } });

            /*_CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "茄子1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "QuerySpeed", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "CardLaunched", Args = new string[] { "0" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "Done", Args = new string[] { } });*/
        }
        private void _4()
        {
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "彥龍" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "QuerySpeed", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "CardLaunched", Args = new string[] { "0" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "Done", Args = new string[] { } });

            /*_CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "彥龍1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "QuerySpeed", Args = new string[] { } });
            _CommandStrings.Enqueue(new CommandString() { Name = "CardLaunched", Args = new string[] { "0" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "Done", Args = new string[] { } });*/
        }


        internal void Update()
        {
            if (new System.TimeSpan(_Timer.Ticks).TotalSeconds > 0.1)
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
        class Input : Regulus.Utility.ConsoleInput, Regulus.Game.IFramework
        {


            public Input(Regulus.Utility.ConsoleViewer view)
                : base(view)
            {
                // TODO: Complete member initialization
            
            }
            
            void Regulus.Game.IFramework.Launch()
            {
                
            }

            void Regulus.Game.IFramework.Shutdown()
            {
                
            }

            bool Regulus.Game.IFramework.Update()
            {
                base.Update();
                return true;
            }
        }
        static void Main(string[] args)
        {
            var view = new Regulus.Utility.ConsoleViewer();
            var input = new Input(view);

            var application = new Regulus.Project.ExiledPrincesses.Application(view, input);
            application.SelectSystemEvent += _OnSelectSystem;
            application.UserRequesterEvent += _OnUserRequester;
            
            
            Regulus.Game.IFramework app = application;
            app.Launch();
            application.SetLogMessage(Regulus.Utility.Console.LogFilter.All);
            var batchCommander = new BatchCommander(application.Command , view);
            while (app.Update())
            {                
                input.Update();
                //batchCommander.Update();                
            }

            app.Shutdown();
            
       
        }

        private static void _OnUserRequester(Regulus.Game.ConsoleFramework<Regulus.Project.ExiledPrincesses.IUser>.IUserRequester user_requester)
        {
            user_requester.Spawn("1" , true);            
        }

        private static void _OnSelectSystem(Regulus.Game.ConsoleFramework<Regulus.Project.ExiledPrincesses.IUser>.ISystemSelector system_selector)
        {
            system_selector.Use("remoting");
            
        }

        
		
		
    }
}
