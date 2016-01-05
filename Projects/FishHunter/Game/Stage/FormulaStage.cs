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

		private StageMachine _StageMachine;

		public FormulaStage(ISoulBinder binder, ExpansionFeature expansion_feature)
		{
			_Binder = binder;
			_ExpansionFeature = expansion_feature;

			_ZsFishFormulaInitialer = new ZsFishFormulaInitialer(
				expansion_feature.FormulaPlayerRecorder, 
				expansion_feature.FormulaFarmRecorder);
		}

		Value<IFishStage> IFishStageQueryer.Query(Guid player_id, int farm_id)
		{
			var val = new Value<IFishStage>();

			new StageLoadFarmData(_ExpansionFeature.FormulaFarmRecorder).Load(farm_id).OnValue +=
					farm_data =>
					{
						_QueryZsFishStage(player_id, farm_data).OnValue += data => val.SetValue(data);
					};

			return val;
		}

		void IStage.Enter()
		{
			_Binder.Bind<IFishStageQueryer>(this);

			OnDoneEvent += OnDoneEvent;

			_StageMachine = new StageMachine();
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
	}
}
