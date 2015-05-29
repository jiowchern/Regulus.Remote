using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Storage
{
    public class Center : Regulus.Remoting.ICore
    {
        Regulus.Utility.Updater _Update;
        Hall _Hall;
        IStorage _Stroage;
        public Center(IStorage storage)
        {
            _Stroage = storage;
            _Hall = new Hall();
            _Update = new Regulus.Utility.Updater();
        }
        void Regulus.Remoting.ICore.ObtainBinder(Regulus.Remoting.ISoulBinder binder)
        {
            _Hall.PushUser(new User(binder, _Stroage));
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Update.Working();
            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Update.Add(_Hall);
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Update.Shutdown();
        }
    }
}
