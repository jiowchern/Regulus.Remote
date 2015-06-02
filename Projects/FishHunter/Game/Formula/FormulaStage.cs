using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGame.Project.FishHunter.Formula;

namespace VGame.Project.FishHunter.Stage
{
    class FormulaStage : Regulus.Utility.IStage, IFishStageQueryer
    {
        public event Regulus.Game.DoneCallback DoneEvent;
        private Regulus.Remoting.ISoulBinder _Binder;

        public FormulaStage(Regulus.Remoting.ISoulBinder binder)
        {
            // TODO: Complete member initialization
            this._Binder = binder;
        }

        void Regulus.Utility.IStage.Enter()
        {
            _Binder.Bind<IFishStageQueryer>(this);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Binder.Unbind<IFishStageQueryer>(this);
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }

        Regulus.Remoting.Value<IFishStage> IFishStageQueryer.Query(long player_id, byte fish_stage)
        {
            if (fish_stage == 200)
                return null;
            var stage = new FishStage(player_id, fish_stage); 
            return stage;
        }

        
    }
}
