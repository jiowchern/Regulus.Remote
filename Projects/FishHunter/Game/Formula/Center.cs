using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{
    public class Center : Regulus.Utility.ICore
    {
        Regulus.Utility.Updater _Update;
        Hall _Hall;

        public Center()
        {
            _Hall = new Hall();
            _Update = new Regulus.Utility.Updater();
        }
        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            _Hall.PushUser(new User(binder));
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Update.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Update.Add(_Hall);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Update.Shutdown();
        }
    }
}
