using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGame.Project.FishHunter.Storage
{
    public class Application : Regulus.Utility.IUpdatable, Regulus.Utility.Console.IViewer, Regulus.Utility.Console.IInput
    {
        public Regulus.Framework.Client<IUser> Client {get ; private set;}
        Regulus.Utility.Updater _Updater;
        public delegate void UserCallback(IUser usee);
        public event UserCallback UserEvent;
        Regulus.Framework.IUserFactoty<IUser> _UserFactory;

        public Application(Regulus.Framework.IUserFactoty<IUser> custom)
        {
            _UserFactory = custom;
            Client = new Regulus.Framework.Client<IUser>(this, this);
            Client.ModeSelectorEvent += _Client_ModeSelectorEvent;
            _Updater = new Regulus.Utility.Updater();
        }
        public Application()
        {
            _UserFactory = new RemotingFactory();
            Client = new Regulus.Framework.Client<IUser>(this, this);
            Client.ModeSelectorEvent += _Client_ModeSelectorEvent;
            _Updater = new Regulus.Utility.Updater();
        }

        void _Client_ModeSelectorEvent(Regulus.Framework.GameModeSelector<IUser> selector)
        {
            selector.AddFactoty("remoting", _UserFactory);
            var userProvider = selector.CreateUserProvider("remoting");
            var user = userProvider.Spawn("user");

            UserEvent(user);
        }

        public bool Update()
        {
            _Updater.Update();
            return Client.Enable;
        }

        public void Launch()
        {
            _Updater.Add(Client);
        }

        public void Shutdown()
        {
            _Updater.Shutdown();
        }

        void Regulus.Utility.Console.IViewer.WriteLine(string message)
        {
            
        }

        void Regulus.Utility.Console.IViewer.Write(string message)
        {
            
        }

        event Regulus.Utility.Console.OnOutput Regulus.Utility.Console.IInput.OutputEvent
        {
            add {  }
            remove {  }
        }
    }
}
