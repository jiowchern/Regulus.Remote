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
        public BuildStorageControllerStage(Storage.IUser user)
        {
            _User = user;
        }
        void Regulus.Utility.IStage.Enter()
        {
            
        }

        void Regulus.Utility.IStage.Leave()
        {
            
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
