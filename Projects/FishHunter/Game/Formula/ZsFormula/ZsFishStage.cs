using System;
using System.Collections.Generic;
using System.Linq;

using NLog;
using NLog.Fluent;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds;

namespace VGame.Project.FishHunter.Formula.ZsFormula
{
	internal class ZsFishStage : IFishStage
	{
		private event Action<HitResponse[]> _OnTotalHitResponseEvent;

		

		private readonly Guid _AccountId;

		private readonly FishFarmData _FishFarmData;

		private readonly IFormulaFarmRecorder _FormulaFarmRecorder;

		private readonly FormulaPlayerRecord _FormulaPlayerRecord;

		private readonly IFormulaPlayerRecorder _FormulaPlayerRecorder;

		private Action<string> _OnHitException;

		public ZsFishStage(Guid account_id, FishFarmData fish_farm_data, FormulaPlayerRecord formula_player_record, IFormulaPlayerRecorder formula_player_recorder, IFormulaFarmRecorder formula_stage_data_recorder)
		{
			_AccountId = account_id;
			_FishFarmData = fish_farm_data;

			_FormulaFarmRecorder = formula_stage_data_recorder;
			_FormulaPlayerRecorder = formula_player_recorder;

			_FormulaPlayerRecord = formula_player_record;
		}

		event Action<string> IFishStage.OnHitExceptionEvent
		{
			add
			{
				_OnHitException += value;
			}

			remove
			{
				_OnHitException -= value;
			}
		}

		event Action<HitResponse[]> IFishStage.OnTotalHitResponseEvent
		{
			add
			{
				_OnTotalHitResponseEvent += value;
			}

			remove
			{
				_OnTotalHitResponseEvent -= value;
			}
		}

		Guid IFishStage.AccountId => _AccountId;

		int IFishStage.FishStage => _FishFarmData.FarmId;

		void IFishStage.Hit(HitRequest request)
		{
			_SerializeRule(request);

			if(!_CheckDataLegality(request))
			{
				return;
			}

			var hitResponses = new ZsHitChecker(_FishFarmData, _FormulaPlayerRecord).TotalRequest(request);

			_FormulaFarmRecorder.Save(_FishFarmData);
			_FormulaPlayerRecorder.Save(_FormulaPlayerRecord);

			_OnTotalHitResponseEvent?.Invoke(hitResponses);

			_MakeLog(request, hitResponses);
		}

		/// <summary>
		///     因為Serialize 無法實現空array，因應下面邏輯判斷，
		///     如果是null 就產生空array
		/// </summary>
		/// <param name="request"></param>
		private void _SerializeRule(HitRequest request)
		{
			foreach(var fishData in request.FishDatas.Where(fish_data => fish_data.GraveGoods == null))
			{
				fishData.GraveGoods = new RequsetFishData[0];
			}
		}

		/// <summary>
		///     檢查資料合法性
		/// </summary>
		private bool _CheckDataLegality(HitRequest request)
		{
			if(request.FishDatas.Any(x => x.FishOdds <= 0))
			{
				this._OnHitException.Invoke("FishData.Odds 不可為0");
				Singleton<Log>.Instance.WriteInfo("FishData.Odds 不可為0");

				LogManager.GetCurrentClassLogger()
						.Fatal("FishData.Odds 不可為0");

				return false;
			}

			if(request.FishDatas.Any(x => x.FishStatus < FISH_STATUS.NORMAL || x.FishStatus > FISH_STATUS.FREEZE))
			{
				this._OnHitException.Invoke("FishData.FishStatus 無此狀態");
				Singleton<Log>.Instance.WriteInfo("FishData.FishStatus 無此狀態");

				LogManager.GetCurrentClassLogger()
						.Fatal("FishData.FishStatus 無此狀態");
				return false;
			}

			if(request.FishDatas.Where(x => x.FishStatus == FISH_STATUS.KING)
					.Any(fishdata => fishdata.FishType < FISH_TYPE.TROPICAL_FISH || fishdata.FishType > FISH_TYPE.WHALE_COLOR))
			{
				this._OnHitException.Invoke("此魚不可為魚王");

				Singleton<Log>.Instance.WriteInfo("此魚不可為魚王");

				LogManager.GetCurrentClassLogger()
						.Fatal("此魚不可為魚王");

				return false;
			}

			if(request.FishDatas.Length == 0)
			{
				this._OnHitException.Invoke("FishDatas長度 不可為0");
				Singleton<Log>.Instance.WriteInfo("FishDatas長度 不可為0");

				LogManager.GetCurrentClassLogger()
						.Fatal("FishDatas長度 不可為0");
				return false;
			}

			if(request.WeaponData.TotalHits <= 0)
			{
				this._OnHitException.Invoke("WeaponData.TotalHitOdds 不可為0");
				Singleton<Log>.Instance.WriteInfo("WeaponData.TotalHitOdds 不可為0");

				LogManager.GetCurrentClassLogger()
						.Fatal("WeaponData.TotalHitOdds 不可為0");
				return false;
			}

			if(request.WeaponData.WeaponType < WEAPON_TYPE.NORMAL || request.WeaponData.WeaponType > WEAPON_TYPE.FREEZE_BOMB)
			{
				this._OnHitException.Invoke("WeaponData.WeaponType，無此編號");
				Singleton<Log>.Instance.WriteInfo("WeaponData.WeaponType，無此編號");

				LogManager.GetCurrentClassLogger()
						.Fatal("WeaponData.WeaponType，無此編號");
				return false;
			}

			if(request.WeaponData.WeaponBet <= 0 || request.WeaponData.WeaponBet > 10000)
			{
				LogManager.GetCurrentClassLogger()
						.Fatal("WeaponData.WeaponBet，押注分數錯誤");

				var msg = $"WeaponData.WeaponBet = {request.WeaponData.WeaponBet}，押注分數錯誤";
				this._OnHitException.Invoke(msg);
				Singleton<Log>.Instance.WriteInfo(msg);
				return false;
			}

			if(request.WeaponData.WeaponOdds != 1)
			{
				this._OnHitException.Invoke("WeaponData.OddsResult，目前只能為1");
				Singleton<Log>.Instance.WriteInfo("WeaponData.OddsResult，目前只能為1");

				LogManager.GetCurrentClassLogger()
						.Fatal("WeaponData.OddsResult，目前只能為1");
				return false;
			}

			if(request.WeaponData.TotalHits != request.FishDatas.Length)
			{
				_OnHitException.Invoke("WeaponData.TotalHits，擊中數量不符");
				Singleton<Log>.Instance.WriteInfo("WeaponData.TotalHits，擊中數量不符");

				LogManager.GetCurrentClassLogger()
						.Fatal("WeaponData.TotalHits，擊中數量不符");
				return false;
			}

			return true;
		}

		private void _MakeLog(HitRequest request, IEnumerable<HitResponse> responses)
		{
			_MakeRequestLog(request);
			_MakeResponseLog(responses);
		}

		private void _MakeRequestLog(HitRequest request)
		{
			foreach(var data in request.FishDatas)
			{
				var log = LogManager.GetLogger("Request");
				log.Info()
					.Message("Request Data")
					.Property("FarmId", _FishFarmData.FarmId)
					.Property("PlayerId", _AccountId)
					.Property("BulletId", request.WeaponData.BulletId)
					.Property("WeaponType", request.WeaponData.WeaponType)
					.Property("Bet", request.WeaponData.WeaponBet)
					.Property("Odds", request.WeaponData.WeaponOdds)
					.Property("Hits", request.WeaponData.TotalHits)
					.Property("FishId", data.FishId)
					.Property("FishType", data.FishType)
					.Property("FishStatus", data.FishStatus)
					.Property("FishOdds", data.FishOdds)
					.Write();
			}
		}

		private void _MakeResponseLog(IEnumerable<HitResponse> responses)
		{
			foreach(var response in responses)
			{
				foreach(var weapon in response.FeedbackWeapons)
				{
					var log = LogManager.GetLogger("Response");
					log.Info()
						.Message("Response Data")
						.Property("FarmId", _FishFarmData.FarmId)
						.Property("PlayerId", _AccountId)
						.Property("BulletId", response.WepId)
						.Property("FishId", response.FishId)
						.Property("FishOdds", response.FishOdds)
						.Property("DieResult", response.DieResult)
						.Property("Bet", response.WeaponBet)
						.Property("OddsResult", response.OddsResult)
						.Property("DieRate", response.DieRate)
						.Property("DieRatePrecent", $"{response.DieRate / (float)0x10000000:p}")
						.Property("Feedback", weapon)
						.Write();
				}
			}
		}
	}
}
