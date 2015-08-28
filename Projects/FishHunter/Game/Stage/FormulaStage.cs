using System;
using System.Collections.Generic;


using Regulus.Game;
using Regulus.Remoting;
using Regulus.Utility;
using Regulus.Extension;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;
using VGame.Project.FishHunter.Formula.ZsFormula;

namespace VGame.Project.FishHunter.Stage
{
    internal class FormulaStage : IStage, IFishStageQueryer
    {
        public event DoneCallback OnDoneEvent;

        private readonly ISoulBinder _Binder;

        private ExpansionFeature _ExpansionFeature;

        private List<FishFarmData> _FishFarmDatas;

        private StageMachine _StageMachine;


        private ZsFishFormulaInitialer _ZsFishFormulaInitialer;
        public FormulaStage(ISoulBinder binder, ExpansionFeature expansion_feature)
        {
            _Binder = binder;
            _ExpansionFeature = expansion_feature;

            _ZsFishFormulaInitialer= new ZsFishFormulaInitialer(expansion_feature.FormulaPlayerRecorder , expansion_feature.FormulaFarmRecorder);
        }

        Value<IFishStage> IFishStageQueryer.Query(Guid player_id, int fish_stage)
        {
            switch(fish_stage)
            {
                case 100:
                    var data = _FishFarmDatas.Find(x => x.FarmId == fish_stage);
                    return _QueryZsFishStage(player_id , data);
                /*case 100:
                    var data = _FishFarmDatas.Find(x => x.FarmId == fish_stage);
                    return new ZsFishStage(
                        player_id, 
                        data, 
                        _ExpansionFeature.FormulaPlayerRecorder, 
                        _ExpansionFeature.FormulaFarmRecorder);*/

                case 111:
                    return new QuarterStage(player_id, fish_stage);

                default:
                    return new FishStage(player_id, fish_stage);
            }
        }

        private Value<IFishStage> _QueryZsFishStage(Guid player_id, FishFarmData data)
        {
            return _ZsFishFormulaInitialer.Query(player_id, data);
        }

        void IStage.Enter()
        {
            OnDoneEvent += OnDoneEvent;

            _StageMachine = _CreateStage();
        }

        void IStage.Leave()
        {
            _Binder.Unbind<IFishStageQueryer>(this);

            _StageMachine.Termination();
            _StageMachine = null;

            OnDoneEvent.Invoke();
        }

        void IStage.Update()
        {
            _StageMachine.Update();
        }

        private StageMachine _CreateStage()
        {
            var stageMachine = new StageMachine();

            var stage = new StageLoadFarmData(_ExpansionFeature.FormulaFarmRecorder);

            stage.OnDoneEvent += _StageLoadFinish; 

            stageMachine.Push(stage);

            return stageMachine;
        }

        private void _StageLoadFinish(List<FishFarmData> obj)
        {
            _FishFarmDatas = obj;

            _Binder.Bind<IFishStageQueryer>(this);
        }
    }

    internal class ZsFishFormulaInitialer
    {
        private IFormulaPlayerRecorder _FormulaPlayerRecorder;

        private IFormulaFarmRecorder _FormulaFarmRecorder;
        public ZsFishFormulaInitialer(IFormulaPlayerRecorder formula_player_recorder, IFormulaFarmRecorder i_formula_farm_recorder)
        {
            _FormulaPlayerRecorder = formula_player_recorder;
            _FormulaFarmRecorder = i_formula_farm_recorder;
        }

        public Value<IFishStage> Query(Guid player_id, FishFarmData data)
        {
            var val = new Value<IFishStage>();
            _FormulaPlayerRecorder.Query(player_id).OnValue += record =>
            {
                val.SetValue(new ZsFishStage(
                        player_id,
                        data,
                        record,
                        _FormulaPlayerRecorder,
                        _FormulaFarmRecorder));
            };

            return val;
        }
    }
}
