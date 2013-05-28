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
            Samebest.Utility.Singleton<Regulus.Utility.ConsoleLogger>.Instance.Launch("TurnBasedRPG");

            Samebest.Game.FrameworkRoot frameworkRoot = new Samebest.Game.FrameworkRoot();

            bool userRunning = true;
            var user = _CreateUser(frameworkRoot);
            user.Quit += () =>
            {
                userRunning = false;
            };
            _CreateRandomBot(frameworkRoot);

            _CreateStageBot(frameworkRoot);

            while (userRunning)
            {
                frameworkRoot.Update();
            }
            frameworkRoot.Shutdown();
            Console.WriteLine("系統關閉...");
            Samebest.Utility.Singleton<Regulus.Utility.ConsoleLogger>.Instance.Shutdown();
            Console.WriteLine("關閉完成.");
        }

        private UserController _CreateUser(Samebest.Game.FrameworkRoot frameworkRoot)
        {
            Console.WriteLine("建立使用者.");
            UserController user = new UserController();
            
            frameworkRoot.AddFramework(user);
            return user;
        }

        private void _CreateStageBot(Samebest.Game.FrameworkRoot frameworkRoot)
        {
            Console.WriteLine("建立狀態機器人...");
            Console.Write("輸入數量:");
            int botCount = 0;
            if (int.TryParse(Console.ReadLine(), out botCount))
            {
                for (int idx = 0; idx < botCount; ++idx)
                {
                    var bot = new StatusBotController("StatusBot" + idx.ToString() );
                    frameworkRoot.AddFramework(bot);
                }
            }
        }
        private void _CreateRandomBot(Samebest.Game.FrameworkRoot frameworkRoot)
        {
            if (System.IO.File.Exists("BotCommand.txt"))
            {
                Console.WriteLine("建立隨機機器人...");
                Console.Write("輸入數量:");
                int botCount = 0;
                if (int.TryParse(Console.ReadLine(), out botCount))
                { 
                    for(int idx = 0; idx < botCount ;++idx)
                    {
                        var bot = new RandomBotController("BotCommand.txt");
                        frameworkRoot.AddFramework(bot);
                    }
                }
            }
        }    
    }
}
