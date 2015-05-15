using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    public class Center : Regulus.Utility.ICore
    {
        Regulus.Utility.CenterOfUpdateable _Updater;
        Hall _Hall;
        VGame.Project.FishHunter.IAccountFinder _AccountFinder;
        
        private IFishStageQueryer _FishStageQueryer;
        

        public Center(IAccountFinder accountFinder, IFishStageQueryer fishStageQueryer)
        {
            _AccountFinder = accountFinder;
            _Updater = new Regulus.Utility.CenterOfUpdateable();
            _Hall = new Hall();            
            
            this._FishStageQueryer = fishStageQueryer;
        }
        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            var user = new User(binder, _AccountFinder, _FishStageQueryer);
            _Hall.PushUser(user);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
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
