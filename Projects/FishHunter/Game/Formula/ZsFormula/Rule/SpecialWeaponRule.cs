using System.Collections.Generic;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     特殊武器的處理
	/// </summary>
	public class SpecialWeaponRule
	{
		private readonly HitRequest _Request;

		private readonly StageDataVisitor _StageVisitor;

		private List<HitResponse> _HitResponses;

		public SpecialWeaponRule(StageDataVisitor stage_visitor, HitRequest request)
		{
			_StageVisitor = stage_visitor;
			_Request = request;
		}

		public HitResponse[] Run()
		{
			return null;

			//			foreach(var fishData in _Request.FishDatas)
			//			{
			//				
			//				var specialWeapon = _StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusStageData.StageId)
			//				                                 .SpecialWeaponDatas.Find(x => x.WeaponType == _Request.WeaponData.WeaponType);
			//
			//				var gate = (int)specialWeapon.Power; // 特武威力
			//
			//				var gate2 = 0;
			//
			//				gate *= 0x0FFFFFFF;
			//
			//				gate /= _Request.WeaponData.TotalHitOdds; // 总倍数
			//
			//				var bufferData = _StageVisitor.FocusStageData.FindBuffer(_StageVisitor.FocusBufferBlock, StageBuffer.BUFFER_TYPE.NORMAL);
			//
			//				var oddsRule = new OddsRuler(fishData, bufferData).RuleResult();
			//
			//				gate /= oddsRule;
			//
			//				if(gate > 0x0FFFFFFF)
			//				{
			//					gate2 = 0x10000000; // > 100%
			//				}
			//				else
			//				{
			//					gate2 = gate;
			//				}
			//
			//				if(_Request.WeaponData.WeaponType == WEAPON_TYPE.BIG_OCTOPUS_BOMB)
			//				{
			//					gate2 = 0x10000000; // > 100% 
			//				}
			//
			//				if(Random.Instance.NextInt(0, 0x10000000) >= gate2)
			//				{
			//					_Miss(fishData, _Request.WeaponData);
			//					continue;
			//				}
			//
			//				var bet = _Request.WeaponData.WepBet * _Request.WeaponData.WepOdds;
			//				var win = fishData.FishOdds * bet * oddsRule;
			//
			//				new SaveScoreHistory(_StageVisitor, win).Run();
			//
			//				_Die(fishData, _Request.WeaponData);
			//			}
			//
			//			_StageVisitor.FormulaStageDataRecorder.Save(_StageVisitor.FocusStageData);
			//			_StageVisitor.FormulaPlayerRecorder.Save(_StageVisitor.PlayerRecord);
			//			return _HitResponses.ToArray();
		}

		private void _Die(RequsetFishData fish_data, RequestWeaponData weapon_data)
		{
			var bufferData = _StageVisitor.FocusStageData.FindBuffer(_StageVisitor.FocusBufferBlock, StageBuffer.BUFFER_TYPE.NORMAL);

			_HitResponses.Add(
				new HitResponse
			{
				WepID = weapon_data.WepID, 
				FishID = fish_data.FishID, 
				DieResult = FISH_DETERMINATION.DEATH, 
				FeedbackWeaponType = new[]
				{
					WEAPON_TYPE.ELECTRIC_NET, 
					WEAPON_TYPE.DAMAGE_BALL
				}, 
				WUp = new OddsRuler(fish_data, bufferData).RuleResult()
			});
		}

		private void _Miss(RequsetFishData fish_data, RequestWeaponData weapon_data)
		{
			var bufferData = _StageVisitor.FocusStageData.FindBuffer(_StageVisitor.FocusBufferBlock, StageBuffer.BUFFER_TYPE.NORMAL);
			_HitResponses.Add(
				new HitResponse
			{
				WepID = weapon_data.WepID, 
				FishID = fish_data.FishID, 
				DieResult = FISH_DETERMINATION.SURVIVAL, 
				FeedbackWeaponType = new[]
				{
					WEAPON_TYPE.INVALID
				}, 
				WUp = 1
			});
		}
	}
}
