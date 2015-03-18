using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaUserBot
{
    class Program
    {
        private static string IPAddress = "210.65.10.160";
        //private static string IPAddress = "127.0.0.1";
        private static int Port = 38971;
        static void Main(string[] args)
        {
            System.Threading.SpinWait sw = new System.Threading.SpinWait();
            var clientHandler = new ClientHandler(IPAddress, Port , 1);
            var view = new Regulus.Utility.ConsoleViewer();
            Log.Instance.SetView(view);
            var input = new Regulus.Utility.ConsoleInput(view);
            var client = new VGame.Project.FishHunter.Formula.Client(view, input);
            client.ModeSelectorEvent += clientHandler.Begin;


            var updater = new Regulus.Utility.Updater();
            updater.Add(client);
            updater.Add(clientHandler);
            
            while (client.Enable)
            {
                input.Update();
                updater.Update();
                sw.SpinOnce();
            }

            updater.Shutdown();
            clientHandler.End();
        }

        private static void _OnSelector(Regulus.Framework.GameModeSelector<VGame.Project.FishHunter.Formula.IUser> selector)
        {
            
            selector.AddFactoty("remoting", new VGame.Project.FishHunter.Formula.RemotingUserFactory());
            _OnProvider(selector.CreateUserProvider("remoting"));
            
        }

        private static void _OnProvider(Regulus.Framework.UserProvider<VGame.Project.FishHunter.Formula.IUser> userProvider)
        {
            _OnUser(userProvider.Spawn("this"));
            userProvider.Select("this");
            
        }

        private static void _OnUser(VGame.Project.FishHunter.Formula.IUser user)
        {
            user.Remoting.ConnectProvider.Supply += _Connect;
        }

        static void _Connect(Regulus.Utility.IConnect obj)
        {
            obj.Connect(IPAddress, Port);
        }

        
    }
}
