using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    class BuildCenterStage : Regulus.Utility.IStage
    {
        public struct ExternalFeature
        {
            public IFishStageQueryer FishStageQueryer;
            public IAccountFinder AccountFinder;
            public IRecordQueriers RecordQueriers;
        }
        ExternalFeature _Feature;
        private Formula.IUser _FormulaUser;
        private Storage.IUser _StorageUser;
        public delegate void SuccessBuiledCallback(ExternalFeature features);
        public event SuccessBuiledCallback BuiledEvent;
        public BuildCenterStage(Formula.IUser _FormulaUser, Storage.IUser _StorageUser)
        {
            _Feature = new ExternalFeature();
            this._FormulaUser = _FormulaUser;
            this._StorageUser = _StorageUser;
        }

        void Regulus.Utility.IStage.Enter()
        {
            _FormulaUser.FishStageQueryerProvider.Supply += _GetFishStageQuery;
        }

        void _GetFishStageQuery(IFishStageQueryer obj)
        {
            _FormulaUser.FishStageQueryerProvider.Supply -= _GetFishStageQuery;
            _Feature.FishStageQueryer = obj;

            _StorageUser.QueryProvider<IAccountFinder>().Supply += _AccountFinder;
        }

        void _AccountFinder(IAccountFinder obj)
        {
            _StorageUser.QueryProvider<IAccountFinder>().Supply -= _AccountFinder;
            _Feature.AccountFinder = obj;


            _StorageUser.QueryProvider<IRecordQueriers>().Supply += _RecordQueriers;
            
        }

        private void _RecordQueriers(IRecordQueriers obj)
        {
            _StorageUser.QueryProvider<IRecordQueriers>().Supply -= _RecordQueriers;
            _Feature.RecordQueriers = obj;
            BuiledEvent(_Feature);
        }

        void Regulus.Utility.IStage.Leave()
        {
            
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
