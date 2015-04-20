using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGame.Project.FishHunter.Formula;

namespace VGame.Project.FishHunter
{
    class BuildStorageControllerStage : Regulus.Utility.IStage
    {
        public delegate void DoneCallback(StorageController controller);
        public event DoneCallback DoneEvent;
        StorageController _Controller;
        Storage.IUser _User;

        IAccountFinder _Finder;
        public BuildStorageControllerStage(Storage.IUser user)
        {
            _User = user;
        }
        void Regulus.Utility.IStage.Enter()
        {
            _User.QueryProvider<IAccountFinder>().Supply += _GetFinder;
        }

        private void _GetFinder(IAccountFinder obj)
        {
            _Finder = obj;

            _Controller = new StorageController(_Finder);
            DoneEvent(_Controller);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _User.QueryProvider<IAccountFinder>().Supply -= _GetFinder;
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
