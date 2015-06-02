using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    class SelectStage : Regulus.Utility.IStage, VGame.Project.FishHunter.ILevelSelector
    {
        private Regulus.Remoting.ISoulBinder _Binder;
        private IFishStageQueryer _FishStageQueryer;


        public delegate void DoneCallback(IFishStage fish_stage);
        public event DoneCallback DoneEvent;

        bool _Querying;
        int[] _Stages;
        public SelectStage(int[] stages , Regulus.Remoting.ISoulBinder binder, IFishStageQueryer fish_stag_queryer)
        {      
      
            this._Binder = binder;
            this._FishStageQueryer = fish_stag_queryer;
            _Querying = false;
            _Stages = stages;
        }
        
        void Regulus.Utility.IStage.Enter()
        {
            _Binder.Bind<VGame.Project.FishHunter.ILevelSelector>(this);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Binder.Unbind<VGame.Project.FishHunter.ILevelSelector>(this);
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }

        Regulus.Remoting.Value<bool> ILevelSelector.Select(int level)
        {
            if (_Check(level) == false)
                return false;

            if(_Querying == false)
            {
                _Querying = true;
                Regulus.Remoting.Value<bool> val = new Regulus.Remoting.Value<bool>();
                checked
                {
                    _FishStageQueryer.Query(0, (byte)level).OnValue += (fish_stage) =>
                    {
                        if (fish_stage != null)
                        {
                            DoneEvent(fish_stage);
                            val.SetValue(true);
                        }
                        else
                        {
                            val.SetValue(false);
                        }
                        _Querying = false;
                    };
                }
                

                return val;
            }
            return false;
        }

        private bool _Check(int level)
        {
            return _Stages.Any((stage) => stage == level);
        }

        Regulus.Remoting.Value<int[]> ILevelSelector.QueryStages()
        {
            return _Stages;
        }
    }
}
