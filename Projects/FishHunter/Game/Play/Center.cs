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

        IAccountFinder _AccountFinder;
        IFishStageQueryer _FishStageQueryer;
        IRecordQueriers _RecordQueriers;
        ITradeNotes _Tradefinder;
        
        public Center(IAccountFinder accountFinder, IFishStageQueryer fishStageQueryer ,IRecordQueriers rq, ITradeNotes tradeAccount)
        {
            _RecordQueriers = rq;
            _AccountFinder = accountFinder;
            _FishStageQueryer = fishStageQueryer;
            _Tradefinder = tradeAccount;
            
            _Updater = new Regulus.Utility.Updater();
            _Hall = new Hall();            
        }

        void Regulus.Remoting.ICore.AssignBinder(Regulus.Remoting.ISoulBinder binder)
        {
            var user = new User(binder, 
                _AccountFinder, 
                _FishStageQueryer,
                _RecordQueriers,
                _Tradefinder);

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
