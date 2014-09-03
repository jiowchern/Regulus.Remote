using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Game
{
    public class UserController<TUser> : Regulus.Game.Framework<TUser>.IController
        where TUser : Regulus.Utility.IUpdatable
    {
        Regulus.Utility.Updater _Updater;
        
        TUser _User;


        public UserController(TUser user)
        {
            _User = user;
            _Updater = new Utility.Updater();
        }


        string _Name;
        string Game.Framework<TUser>.IController.Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }


        public delegate void OnLook(TUser user);
        public event OnLook LookEvent;
        public event OnLook UnlookEvent;
        void Game.Framework<TUser>.IController.Look()
        {
            if(LookEvent != null)
                LookEvent(_User);
        }

        void Game.Framework<TUser>.IController.NotLook()
        {
            if(UnlookEvent!= null)
                UnlookEvent(_User);
        }

        bool Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {

            _Updater.Add(_User);
        }

        void Framework.ILaunched.Shutdown()
        {

            _Updater.Shutdown();
        }


        TUser Game.Framework<TUser>.IController.GetUser()
        {
            return _User;
        }
    }
}
