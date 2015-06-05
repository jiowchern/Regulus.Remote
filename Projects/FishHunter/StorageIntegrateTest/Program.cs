using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageIntegrateTest
{
    class Program
    {
        static bool _Enable;
        static void Main(string[] args)
        {
            System.Threading.SpinWait sw = new System.Threading.SpinWait();
            _Enable = true;
            Regulus.Utility.Updater updater = new Regulus.Utility.Updater();
            Regulus.Utility.Launcher launcher = new Regulus.Utility.Launcher();


            VGame.Project.FishHunter.Storage.Server server = new VGame.Project.FishHunter.Storage.Server();
            Regulus.Remoting.Soul.Native.Server serverAppliction = new Regulus.Remoting.Soul.Native.Server(server , 12345);
            
            
            VGame.Project.FishHunter.Storage.Proxy client = new VGame.Project.FishHunter.Storage.Proxy();
            client_UserEvent(client.SpawnUser("user"));

            launcher.Push(serverAppliction);
            updater.Add(client);

            launcher.Launch();
            while (_Enable)
            {
                updater.Working();
                sw.SpinOnce();
                
            }
            updater.Shutdown();
            launcher.Shutdown();

            Console.ReadKey();
        }

        static void client_UserEvent(VGame.Project.FishHunter.Storage.IUser usee)
        {
            usee.Remoting.ConnectProvider.Supply += ConnectProvider_Supply;
            usee.VerifyProvider.Supply += VerifyProvider_Supply;
        }

        static void VerifyProvider_Supply(VGame.Project.FishHunter.IVerify obj)
        {
            var result = obj.Login("vgameadmini", "");
            result.OnValue += (val) =>
            {
                if (val)
                    Console.WriteLine("驗證成功");
                else
                {
                    Console.WriteLine("驗證失敗");                    
                }
                _Enable = false;
            };
        }

        static void ConnectProvider_Supply(Regulus.Utility.IConnect obj)
        {
            var result = obj.Connect("127.0.0.1", 12345);
            result.OnValue += (val) => 
            {
                if (val)
                    Console.WriteLine("連線成功");
                else
                {
                    Console.WriteLine("連線失敗");
                    _Enable = false;
                }
                    
            };
        }
    }
}
