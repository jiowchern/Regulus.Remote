using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Storage
{
    public class Center : Regulus.Utility.ICore
    {
        Regulus.Utility.CenterOfUpdateable _Update;
        Hall _Hall;
        IStorage _Stroage;
        public Center(IStorage storage)
        {
            _Stroage = storage;
            _Hall = new Hall();
            _Update = new Regulus.Utility.CenterOfUpdateable();
        }
        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            _Hall.PushUser(new User(binder, _Stroage));
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Update.GetObjectSet();
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
