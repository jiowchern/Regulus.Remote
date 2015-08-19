using System;
using System.Collections.Generic;


using Regulus.Extension;
using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;


using Random = Regulus.Utility.Random;

namespace VGame.Project.FishHunter.Formula.ZsFormula
{
	internal class ZsFishStage : IFishStage
	{
		private event Action<HitResponse[]> _OnTotalHitResponseEvent;

		private readonly Guid _AccountId;

		private readonly FishFarmData _FishFarmData;

		private readonly IFormulaFarmRecorder _FormulaFarmRecorder;

		private readonly IFormulaPlayerRecorder _FormulaPlayerRecorder;

		private FormulaPlayerRecord _FormulaPlayerRecord;

		private Action<string> _OnHitExceptionEvent;

		public ZsFishStage(Guid account_id, FishFarmData fish_farm_data)
		{
			_AccountId = account_id;

			_FishFarmData = fish_farm_data;
		}

		public ZsFishStage(
			Guid account_id, 
			FishFarmData fish_farm_data, 
			IFormulaPlayerRecorder formula_player_recorder, 
			IFormulaFarmRecorder formula_stage_data_recorder)
		{
			_AccountId = account_id;
			_FishFarmData = fish_farm_data;

			_FormulaFarmRecorder = formula_stage_data_recorder;
			_FormulaPlayerRecorder = formula_player_recorder;

			_StroageLoad(account_id).OnValue += obj => { _FormulaPlayerRecord = obj; };
		}

		event Action<string> IFishStage.OnHitExceptionEvent
		{
			add { _OnHitExceptionEvent += value; }
			remove { _OnHitExceptionEvent -= value; }
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
			get { return _FishFarmData.FarmId; }
		}

		void IFishStage.Hit(HitRequest request)
		{

			if(request.WeaponData.TotalHitOdds <= 0)
			{
				_OnHitExceptionEvent.Invoke("WeaponData.TotalHitOdds 此數值不可為0");
				Singleton<Log>.Instance.WriteInfo("WeaponData.TotalHitOdds 此數值不可為0");
				return;
			}

			var totalRequest = new ZsHitChecker(_FishFarmData, _FormulaPlayerRecord, Random.Instance).TotalRequest(request);

			_FormulaFarmRecorder.Save(_FishFarmData);
			_FormulaPlayerRecorder.Save(_FormulaPlayerRecord);

			_OnTotalHitResponseEvent.Invoke(totalRequest);

			_MakeLog(request, totalRequest);
		}

		private Value<FormulaPlayerRecord> _StroageLoad(Guid account_id)
		{
			var returnValue = new Value<FormulaPlayerRecord>();

			var val = _FormulaPlayerRecorder.Query(account_id);

			val.OnValue += gamePlayerRecord => { returnValue.SetValue(gamePlayerRecord); };

			return returnValue;
		}

		private void _MakeLog(HitRequest request, IEnumerable<HitResponse> response)
		{
			foreach(var hit in response)
			{
				var format = "PlayerVisitor:{0}\tStage:{1}\nRequest:{2}\nResponse:{3}";

				var log = string.Format(
					format, 
					_AccountId, 
					_FishFarmData.FarmId, 
					request.ShowMembers(" "), 
					hit.ShowMembers(" "));

				Singleton<Log>.Instance.WriteInfo(log);
			}
		}
	}
}
