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

		private readonly List<FishFarmData> _StageDatas;

		private ExpansionFeature _ExpansionFeature;

		public FormulaStage(ISoulBinder binder, ExpansionFeature expansion_feature)
		{
			_Binder = binder;
			_ExpansionFeature = expansion_feature;
			_StageDatas = new List<FishFarmData>();
		}

		Value<IFishStage> IFishStageQueryer.Query(Guid player_id, int fish_stage)
		{
			switch(fish_stage)
			{
				case 100:
					var data = _StageDatas.Find(x => x.FarmId == fish_stage);
					return new ZsFishStage(player_id, data, _ExpansionFeature.FormulaPlayerRecorder, _ExpansionFeature.FormulaFarmRecorder);

				case 111:
					return new QuarterStage(player_id, fish_stage);

				default:
					return new FishStage(player_id, fish_stage);
					
			}
		}

		void IStage.Enter()
		{
			OnDoneEvent += OnDoneEvent;

			_InitStageData();

			_Binder.Bind<IFishStageQueryer>(this);
		}

		void IStage.Leave()
		{
			_Binder.Unbind<IFishStageQueryer>(this);

			OnDoneEvent.Invoke();
		}

		void IStage.Update()
		{
		}

		private Value<FishFarmData> _StroageLoad(int farm_id)
		{
			var returnValue = new Value<FishFarmData>();

			var val = _ExpansionFeature.FormulaFarmRecorder.Load(farm_id);

			val.OnValue += stage_data =>
			{
                Log.Instance.WriteDebug("_StroageLoad");
			    returnValue.SetValue(stage_data);
			};

			return returnValue;
		}

		/// <summary>
		///     戴入所有營業中的魚場資料
		/// </summary>
		private void _InitStageData()
		{
            Singleton<Log>.Instance.WriteDebug("Init farm data.");

            foreach (var t in new BusinessFarm().FarmIds)
			{
				var data = _StroageLoad(t);

				//data.OnValue += data_on_value => _StageDatas.Add(data_on_value);

				var dataOnValue = data.Result();

				_StageDatas.Add(dataOnValue);
				
			}

            Singleton<Log>.Instance.WriteDebug("farm data loading finish");
        }
	}
}
