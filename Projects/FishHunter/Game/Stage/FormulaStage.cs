using System;
using System.Collections.Generic;


using Regulus.Game;
using Regulus.Remoting;
using Regulus.Utility;
using Regulus.Extension;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;

namespace VGame.Project.FishHunter.Stage
{
	internal class FormulaStage : IStage, IFishStageQueryer
	{
		public event DoneCallback OnDoneEvent;

		private readonly ISoulBinder _Binder;

		private readonly ZsFishFormulaInitialer _ZsFishFormulaInitialer;

		private ExpansionFeature _ExpansionFeature;

		private List<FishFarmData> _FishFarmDatas;

		private StageMachine _StageMachine;

		public FormulaStage(ISoulBinder binder, ExpansionFeature expansion_feature)
		{
			_Binder = binder;
			_ExpansionFeature = expansion_feature;

			_ZsFishFormulaInitialer = new ZsFishFormulaInitialer(
				expansion_feature.FormulaPlayerRecorder, 
				expansion_feature.FormulaFarmRecorder);
		}

		Value<IFishStage> IFishStageQueryer.Query(Guid player_id, int fish_stage)
		{
			if(fish_stage >= 100 && fish_stage <= 111)
			{
				var data = _FishFarmDatas.Find(x => x.FarmId == fish_stage);
				return _QueryZsFishStage(player_id, data);
			}
			else if(fish_stage == 200)
			{
				return new QuarterStage(player_id, fish_stage);
			}
			else
			{
				return new FishStage(player_id, fish_stage);
			}
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

		private Value<IFishStage> _QueryZsFishStage(Guid player_id, FishFarmData data)
		{
			return _ZsFishFormulaInitialer.Query(player_id, data);
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
}
