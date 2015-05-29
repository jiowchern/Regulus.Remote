using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    public class Center : Regulus.Remoting.ICore
    {
        Regulus.Utility.Updater _Updater;
        Hall _Hall;
        VGame.Project.FishHunter.IAccountFinder _AccountFinder;
        
        private IFishStageQueryer _FishStageQueryer;
        private IRecordQueriers _RecordQueriers;
        

        public Center(IAccountFinder accountFinder, IFishStageQueryer fishStageQueryer ,IRecordQueriers rq )
        {
            _RecordQueriers = rq;
            _AccountFinder = accountFinder;
            _Updater = new Regulus.Utility.Updater();
            _Hall = new Hall();            
            
            this._FishStageQueryer = fishStageQueryer;
        }
        void Regulus.Remoting.ICore.ObtainBinder(Regulus.Remoting.ISoulBinder binder)
        {
            var user = new User(binder, 
                _AccountFinder, 
                _FishStageQueryer,
                _RecordQueriers
                );
            _Hall.PushUser(user);
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
