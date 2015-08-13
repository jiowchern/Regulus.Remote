using System;
using System.Collections.Generic;


using Regulus.Extension;
using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula.ZsFormula
{
	internal class ZsFishStage : IFishStage
	{
		private event Action<HitResponse> _OnHitResponseEvent;

		private event Action<HitResponse[]> _OnTotalHitResponseEvent;

		private readonly Guid _AccountId;

		private readonly StageData _StageData;

		private Action<string> _OnHitExceptionEvent;

		private IFormulaStageDataRecorder _FormulaStageDataRecorder;

		private IFormulaPlayerRecorder _FormulaPlayerRecorder;

		public ZsFishStage(Guid account_id, StageData stage_data)
		{
			_AccountId = account_id;

			_StageData = stage_data;
		}

		public ZsFishStage(Guid account_id, StageData stage_data, IFormulaPlayerRecorder formula_player_recorder, IFormulaStageDataRecorder formula_stage_data_recorder)
		{
			_AccountId = account_id;
			_StageData = stage_data;

			_FormulaStageDataRecorder = formula_stage_data_recorder;
			_FormulaPlayerRecorder = formula_player_recorder;
		}

		event Action<string> IFishStage.OnHitExceptionEvent
		{
			add { _OnHitExceptionEvent += value; }
			remove { _OnHitExceptionEvent -= value; }
		}

		event Action<HitResponse> IFishStage.OnHitResponseEvent
		{
			add { _OnHitResponseEvent += value; }
			remove { _OnHitResponseEvent -= value; }
		}

		event Action<HitResponse[]> IFishStage.OnTotalHitResponseEvent
		{
			add { _OnTotalHitResponseEvent += value; }
			remove { _OnTotalHitResponseEvent -= value; }
		}

		Guid IFishStage.AccountId
		{
			get { return _AccountId; }
		}

		int IFishStage.FishStage
		{
			get { return _StageData.StageId; }
		}

		void IFishStage.Hit(HitRequest request)
		{
			var totalRequest = new ZsHitChecker(_StageData, _FormulaStageDataRecorder, _FormulaPlayerRecorder).TotalRequest(request);

			_OnTotalHitResponseEvent.Invoke(totalRequest);

			_MakeLog(request, totalRequest);
		}

		private void _MakeLog(HitRequest request, IEnumerable<HitResponse> response)
		{
			foreach(var hit in response)
			{
				var format = "PlayerVisitor:{0}\tStage:{1}\nRequest:{2}\nResponse:{3}";

				var log = string.Format(
					format,
					_AccountId,
					_StageData.StageId,
					request.ShowMembers(" "),
					hit.ShowMembers(" "));

				Singleton<Log>.Instance.WriteInfo(log);
				
			}
			
		}
	}
}
