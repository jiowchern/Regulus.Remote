using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    class SelectStage : Regulus.Utility.IStage
    {
        private Regulus.Remoting.ISoulBinder _Binder;
        private IFishStageQueryer _FishStageQueryer;


        public delegate void DoneCallback(IFishStage fish_stage);
        public event DoneCallback DoneEvent;
            
        public SelectStage(Regulus.Remoting.ISoulBinder binder, IFishStageQueryer fish_stag_queryer)
        {            
            this._Binder = binder;
            this._FishStageQueryer = fish_stag_queryer;
        }
        
        void Regulus.Utility.IStage.Enter()
        {
            _FishStageQueryer.Query(0, 1).OnValue += (fish_stage) => { DoneEvent(fish_stage); };
        }

        void Regulus.Utility.IStage.Leave()
        {
            
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
