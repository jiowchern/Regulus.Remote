using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGame.Project.FishHunter.Storage
{
    public class Proxy :         
        Regulus.Utility.IUpdatable, Regulus.Utility.Console.IViewer, Regulus.Utility.Console.IInput
    {
        Regulus.Framework.Client<IUser> _Client;
        Regulus.Utility.CenterOfUpdateable _Updater;
        
        Regulus.Framework.IUserFactoty<IUser> _UserFactory;

        public Proxy(Regulus.Framework.IUserFactoty<IUser> custom)
        {
            _UserFactory = custom;
            _Client = new Regulus.Framework.Client<IUser>(this, this);            
            _Updater = new Regulus.Utility.CenterOfUpdateable();

            Client_ModeSelectorEvent(_Client.Selector);
        }

        void Client_ModeSelectorEvent(Regulus.Framework.GameModeSelector<IUser> selector)
        {
            selector.AddFactoty("fac", _UserFactory);
            _UserProvider = selector.CreateUserProvider("fac");
        }
        public Proxy()
        {
            _UserFactory = new RemotingFactory();
            _Client = new Regulus.Framework.Client<IUser>(this, this);            
            _Updater = new Regulus.Utility.CenterOfUpdateable();

            Client_ModeSelectorEvent(_Client.Selector);
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

        public IUser SpawnUser(string name)
        {
            return _UserProvider.Spawn(name);
        }

        private Regulus.Framework.UserProvider<IUser> _UserProvider;

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            return _Client.Enable;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Updater.Add(_Client);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Updater.Shutdown();
        }

        public void UnspawnUser(string name)
        {
            _UserProvider.Unspawn(name);
        }
    }
}
