using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPGUserConsole
{
    class Application
    {
        
        internal void Run()
        {
            Console.WriteLine("用摸的RPG");
            Console.WriteLine("系統啟動...");
            //Console.Write("請輸入連線位置&Port (127.0.0.1:5055):");
            //var addr = Console.ReadLine();
            var addr = "114.34.90.217:5055";           
            //var addr = "127.0.0.1:5055";            
            TurnBasedRPG.User user = _Generate(addr);
            user.LinkSuccess += user_LinkSuccess;
            user.LinkFail += user_LinkFail;
            Samebest.Game.IFramework framework = user as Samebest.Game.IFramework;
            

            Regulus.Project.TurnBasedRPGUserConsole.CommandHandler commandHandler = new Regulus.Project.TurnBasedRPGUserConsole.CommandHandler();
            commandHandler.Initialize();
            Regulus.Project.TurnBasedRPGUserConsole.CommandBinder commandBinder = new Regulus.Project.TurnBasedRPGUserConsole.CommandBinder(commandHandler, user);
            commandBinder.Setup();
            

        
            Stack<char> chars = new Stack<char>();
            framework.Launch();
            while (framework.Update())
            {
                if (Console.KeyAvailable)
                {                    
                    string[] command = _HandlerInput(chars);
                    if (command != null)
                        _HandleCommand(commandHandler , command);
                }
            }
            Console.WriteLine("系統關閉...");
            framework.Shutdown();
            commandBinder.TearDown();
            commandHandler.Finialize();
            Console.WriteLine("關閉完成.");
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

        void user_LinkFail()
        {
            Console.WriteLine("連線失敗.");
        }

        void user_LinkSuccess()
        {
            Console.WriteLine("連線成功.");
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
                
                chars.Clear();
                Console.Write("\n");
                
                return commands.Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                chars.Push(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
            return null;
        }

        private TurnBasedRPG.User _Generate(string addr)
        {
            var user = new TurnBasedRPG.User(new Samebest.Remoting.Ghost.Config() { Address = addr, Name = "TurnBasedRPGComplex" });            
            return user;
        }
    }
}
