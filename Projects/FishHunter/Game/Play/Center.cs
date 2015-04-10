using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    public class Center : Regulus.Utility.ICore
    {
        Regulus.Utility.Updater _Updater;
        Hall _Hall;
        VGame.Project.FishHunter.IAccountFinder _Storage;
        public Center(VGame.Project.FishHunter.IAccountFinder storage)
        {
            _Storage = storage;
            _Updater = new Regulus.Utility.Updater();
            _Hall = new Hall();
        }
        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            var user = new User(binder, _Storage);
            _Hall.PushUser(user);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Updater.Add(_Hall);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Updater.Shutdown();
        }
    }
}
