using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
            
            this._Command = command; 

            _Command.Register("1" , _1);
            _Command.Register("2", _2);
            _Command.Register("3", _3);

            _CommandStrings = new Queue<CommandString>();
            
            
            
        
        }

        private void _3()
        {
            throw new NotImplementedException();
        }

        private void _2()
        {
            _CommandStrings.Enqueue(new CommandString() { Name = "remoting", Args = new string[] { } });

            _CommandStrings.Enqueue(new CommandString() { Name = "spawncontroller", Args = new string[] { "jc" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "jc" } });

            _CommandStrings.Enqueue(new CommandString() { Name = "connect", Args = new string[] { "127.0.0.1:5055" } });

            _CommandStrings.Enqueue(new CommandString() { Name = "ready", Args = new string[] { } });

            _CommandStrings.Enqueue(new CommandString() { Name = "login", Args = new string[] { "jc1", "1" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "go", Args = new string[] { } });
        }

        private void _1()
        {
            _CommandStrings.Enqueue(new CommandString() { Name = "standalong", Args = new string[] { } });

            _CommandStrings.Enqueue(new CommandString() { Name = "spawncontroller", Args = new string[] { "jc" } });
            _CommandStrings.Enqueue(new CommandString() { Name = "selectcontroller", Args = new string[] { "jc" } });
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
        class Input : Regulus.Utility.ConsoleInput, Regulus.Utility.IUpdatable
        {


            public Input(Regulus.Utility.ConsoleViewer view)
                : base(view)
            {
                // TODO: Complete member initialization
            
            }



			bool Regulus.Utility.IUpdatable.Update()
			{
				base.Update();
				return true;
			}

			void Regulus.Framework.ILaunched.Launch()
			{
				throw new NotImplementedException();
			}

			void Regulus.Framework.ILaunched.Shutdown()
			{
				throw new NotImplementedException();
			}
		}
        static void Main(string[] args)
        {
            var view = new Regulus.Utility.ConsoleViewer();
            var input = new Input(view);

            var application = new Regulus.Project.ExiledPrincesses.Application(view, input);
            //application.SelectSystemEvent += _OnSelectSystem;
            //application.UserRequesterEvent += _OnUserRequester;
            
            
            Regulus.Utility.IUpdatable app = application;
            app.Launch();
            application.SetLogMessage(Regulus.Utility.Console.LogFilter.All);
            var batchCommander = new BatchCommander(application.Command , view);
            while (app.Update())
            {                
                input.Update();
                batchCommander.Update();                
            }

            app.Shutdown();
            
       
        }

        private static void _OnUserRequester(Regulus.Game.Framework<Regulus.Project.ExiledPrincesses.IUser>.IUserRequester user_requester)
        {
            user_requester.Spawn("1" , true);            
        }

        private static void _OnSelectSystem(Regulus.Game.Framework<Regulus.Project.ExiledPrincesses.IUser>.ISystemSelector system_selector)
        {
            system_selector.Use("remoting");
            
        }

        
		
		
    }
}
