using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.SamebestKeysUserConsole
{
    abstract class Controller : Regulus.Utility.IUpdatable
    {
        Regulus.Project.SamebestKeys.IUser _User;
        protected Regulus.Project.SamebestKeysUserConsole.CommandHandler _CommandHandler;

        Regulus.Project.SamebestKeysUserConsole.CommandBinder _CommandBinder;

        protected abstract void _Launch(Regulus.Project.SamebestKeys.User user);
		void Regulus.Framework.ILaunched.Launch()
        {
            //Console.Write("請輸入連線位置&Port (127.0.0.1:5055):");  
            //var addr = Console.ReadLine();    
            var addr = "114.34.90.217:5055";           
            //var addr = "127.0.0.1:5055"; 
            var user = _GenerateUser(addr);
            user.LinkSuccess += () => { Console.WriteLine("連線成功."); };
            user.LinkFail += (s) => { Console.WriteLine("連線失敗." + s); };
            _User = user;
            _User.Launch();  

            _CommandHandler = new Regulus.Project.SamebestKeysUserConsole.CommandHandler();
            _CommandHandler.Initialize();
            _CommandBinder = new Regulus.Project.SamebestKeysUserConsole.CommandBinder(_CommandHandler, user);
            _CommandBinder.Setup();
            
            _Launch(user);    
        }

        protected abstract string[] _HandlerInput();

		bool Regulus.Utility.IUpdatable.Update()
        {
            _CommandBinder.Update();
            string[] command = _HandlerInput();
            if (command != null)
                _HandleCommand(_CommandHandler, command);

            return _User.Update();
        }
        protected abstract void _Shutdown();

		void Regulus.Framework.ILaunched.Shutdown()
        {
            
            _Shutdown();
            _User.Shutdown();
            _CommandBinder.TearDown();
            _CommandHandler.Finialize();
        }

        private SamebestKeys.User _GenerateUser(string addr)
        {
            
            var user = new SamebestKeys.User(new Regulus.Remoting.Ghost.Config() { Address = addr, Name = "SamebestKeysComplex" });
            return user;
        }
        private void _HandleCommand(CommandHandler command_handler, string[] command)
        {
            if (command.Length > 0)
            {
                var queue = new Queue<string>(command);
                var cmd = queue.Dequeue();
                command_handler.Run(cmd, queue.ToArray());
            }

        }
        
    }

    class RandomBotController : Controller
    {
        Command[] _Commands;
        
        public RandomBotController(string script_path )
        {
            _Commands = _ReadCommand(script_path);
            _Random = new Random(System.DateTime.Now.Millisecond);
        }
        [Regulus.Game.Data.Table("Command")]
        class Command
        {
            [Regulus.Game.Data.Field("Command")]
            public string Content { get; set; }
            [Regulus.Game.Data.Field("Cooldown")]
            public float Cooldown { get; set; }
        }

        private Command[] _ReadCommand(string p)
        {
            Regulus.Game.Data.PrototypeFactory factory = new Regulus.Game.Data.PrototypeFactory();
            factory.LoadCSV("Command", p);
            var cmds = factory.GeneratePrototype<Command>();
            return cmds;
        }

        protected override string[] _HandlerInput()
        {
            Command cmd = _FindCommand();
            if (cmd != null)
            {
                var cmds = cmd.Content.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                var fround = (from label in _CommandLabels where label.ToLower() == cmds[0].ToLower() select true).FirstOrDefault();
                if (fround)
                {
                    return cmds;
                }
                else
                {
                    _Expiry = System.DateTime.Now;
                }
            }
            return null;
        }

        System.DateTime _Expiry;
        Random _Random ;
        private Command _FindCommand()
        {
            var seconds = (System.DateTime.Now - _Expiry);
            if ( seconds.TotalSeconds >= 0)
            {
                var idx = _Random.Next(0,_Commands.Length);
                var cmd = _Commands[idx];                
                _Expiry = System.DateTime.Now;
                _Expiry = _Expiry.AddSeconds(cmd.Cooldown);
                return cmd;                
            }
            return null;
        }

        protected override void _Launch(Regulus.Project.SamebestKeys.User user)
        {

            _Bind(user.VerifyProvider);
            _Bind(user.PlayerProvider);
            _Bind(user.ParkingProvider);
        }

        private void _Bind<T>(Regulus.Remoting.Ghost.IProviderNotice<T> provider_notice)
        {
            provider_notice.Supply += provider_notice_Supply;            
        }

        string[] _CommandLabels = new string[]{};

        void provider_notice_Supply<T>(T obj)
        {            
            var methods = typeof(T).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            _CommandLabels = (from method in methods where method.IsSpecialName == false select method.Name).ToArray();
        }

        protected override void _Shutdown()
        {
            
        }

        
    }

    class UserController : Controller
    {
        
        public UserController()
        { 
        }

        Stack<char> _InputData = new Stack<char>();
        protected override string[] _HandlerInput()
        {
            if (Console.KeyAvailable)
            {
                return _HandlerInput(_InputData);
                
            }
            return null;
        }

        private string[] _HandlerInput(Stack<char> chars)
        {
            var keyInfo = Console.ReadKey(true);
            // Ignore if Alt or Ctrl is pressed.
            if ((keyInfo.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
                return null;
            if ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                return null;
            // Ignore if KeyChar value is \u0000.
            if (keyInfo.KeyChar == '\u0000')
                return null;
            // Ignore tab key.
            if (keyInfo.Key == ConsoleKey.Tab)
                return null;
            if (keyInfo.Key == ConsoleKey.Escape)
                return null;

            if (keyInfo.Key == ConsoleKey.Backspace && chars.Count() > 0)
            {
                chars.Pop();
                Console.Write("\b \b");
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {

                string commands = new string(chars.Reverse().ToArray());
                Regulus.Utility.Singleton<Regulus.Utility.ConsoleLogger>.Instance.Log("Enter Command : " + commands);
                chars.Clear();

                Console.Write("\n");
                return commands.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                chars.Push(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
            return null;
        }

        public event Action Quit;
        protected override void _Launch(Regulus.Project.SamebestKeys.User user)
        {
            _CommandHandler.Set("quit", (cmd) => 
            {
                if (Quit != null)
                    Quit();
            }, "離開 ex. quit");
        }

        protected override void _Shutdown()
        {
            
        }
    }

    class StatusBotController : Controller
    {
        Regulus.Game.StageMachine<StatusBotController> _StageMachine;
        public SamebestKeys.User User {get; private set;}
        public string Name { get; private set; }
        public StatusBotController(string name)
        {
            Name = name;
        }
        protected override void _Launch(SamebestKeys.User user)
        {
            User = user;
            User.LinkFail += User_LinkFail;
            _StageMachine = new Regulus.Game.StageMachine<StatusBotController>(this);
            ToVerify();
        }

        void User_LinkFail(string msg)
        {
            ToConnect();
        }

        protected override void _Shutdown()
        {
            User.LinkFail -= User_LinkFail;
        }

        protected override string[] _HandlerInput()
        {
            _StageMachine.Update();
            return null;
        }

        internal void ToVerify()
        {
            
            _StageMachine.Push(new BotStage.Verify());            
        }

        internal void ToParking()
        {
            
            _StageMachine.Push(new BotStage.Parking());            
        }

        internal void ToGame()
        {
            
            _StageMachine.Push(new BotStage.Game());            
        }
        internal void ToConnect()
        {
            _StageMachine.Push(new BotStage.Connect());            
        }



        internal void ToMove()
        {
            _StageMachine.Push(new BotStage.Move()); 
        }

        internal void ToBodyMovements()
        {
            _StageMachine.Push(new BotStage.BodyMovements()); 
        }
    }
    
}
