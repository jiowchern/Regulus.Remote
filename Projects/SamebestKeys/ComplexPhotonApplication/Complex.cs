using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    public class Complex : Regulus.Game.ICore
    {
        Regulus.Project.SamebestKeys.Hall _Hall;
        World _World;
        LocalTime _Time;
        Regulus.Utility.Updater _Updater;
        IStorage _Storage;

        

        public Complex(IStorage stroage)
        {
            _Time = new LocalTime();
            _Hall = new Hall();
            _World = new World(LocalTime.Instance);
            _Updater = new Utility.Updater();
            _Storage = stroage;
        }

        void Game.ICore.ObtainController(Remoting.ISoulBinder binder)
        {
            _Hall.PushUser(new User(binder, _Hall, _World, _Storage));
        }

        bool Utility.IUpdatable.Update()
        {
            
            _Updater.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _Updater.Add(_Time);
            _Updater.Add(_World);

            _Updater.Add(_Hall);            
        }

        void Framework.ILaunched.Shutdown()
        {
            
            _Updater.Remove(_Hall);

            _Updater.Remove(_World);
            _Updater.Remove(_Time);            
        }
    }
}
