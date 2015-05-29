using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{
    public class Center : Regulus.Remoting.ICore
    {
        Regulus.Utility.Updater _Updater;
        Hall _Hall;
        StorageController _Controller;
        
        private Center()
        {
            _Hall = new Hall();
            _Updater = new Regulus.Utility.Updater();
        }

        public Center(StorageController controller) : this()
        {
            // TODO: Complete member initialization
            this._Controller = controller;
        }

        void Regulus.Remoting.ICore.AssignBinder(Regulus.Remoting.ISoulBinder binder)
        {            
            _Hall.PushUser(new User(binder, _Controller));
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Updater.Add(_Hall);
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Updater.Shutdown();
        }
    }
}
