using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public class Core :Regulus.Utility.ICore
    {
        Regulus.Utility.Updater _Updater;
        Hall _Hall;

        public Core()
        {
            _Updater = new Regulus.Utility.Updater();
            _Hall = new Hall();
        }
        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            var user = new User();
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
