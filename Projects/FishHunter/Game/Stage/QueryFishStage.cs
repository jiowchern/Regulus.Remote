using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Stage
{
    public class QueryFishStage : VGame.Project.FishHunter.IFishStageQueryer , Regulus.Utility.IStage
    {
        private Regulus.Remoting.ISoulBinder _Binder;
        public delegate void FishStageFormulaCallback(long account , int stage , VGame.Project.FishHunter.Formula.HitBase formula);
        public event FishStageFormulaCallback DoneEvent;
        public QueryFishStage(Regulus.Remoting.ISoulBinder binder)
        {            
            this._Binder = binder;
        }

        Regulus.Remoting.Value<bool> IFishStageQueryer.Query(long player_id, byte fish_stage)
        {
            DoneEvent(player_id , fish_stage, new VGame.Project.FishHunter.Formula.HitTest(Regulus.Utility.Random.Instance.R));
            return true;
        }

        void Regulus.Utility.IStage.Enter()
        {
            _Binder.Bind<VGame.Project.FishHunter.IFishStageQueryer>(this);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Binder.Unbind<VGame.Project.FishHunter.IFishStageQueryer>(this);
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
