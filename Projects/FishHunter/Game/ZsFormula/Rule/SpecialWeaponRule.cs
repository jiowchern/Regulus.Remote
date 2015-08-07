using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;


using Random = Regulus.Utility.Random;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     特殊武器的處理
	/// </summary>
	public class SpecialWeaponRule
	{
		private readonly PlayerRecord _PlayerRecord;

		private readonly HitRequest _Request;

		private readonly FishStageVisitor _StageVisitor;

		public SpecialWeaponRule(FishStageVisitor stage_visitor, HitRequest request, PlayerRecord player_record)
		{
			_StageVisitor = stage_visitor;
			_Request = request;
			_PlayerRecord = player_record;
		}

		public HitResponse Run()
		{
			foreach (var fishData in _Request.FishDatas)
			{
				var specialWeapon = _PlayerRecord.FindStageRecord(_StageVisitor.NowData.StageId)
					.SpecialWeaponDatas.Find(x => x.WeaponType == _Request.WeaponData.WeaponType);

				var gate = (int)specialWeapon.Power; // 特武威力

				var gate2 = 0;

				gate *= 0x0FFFFFFF;

				gate /= _Request.WeaponData.TotalHitOdds; // 总倍数

				var bufferData = _StageVisitor.NowData.FindBuffer(_StageVisitor.NowBlock, StageBuffer.BUFFER_TYPE.NORMAL);

				var oddsRule = new OddsRuler(fishData, bufferData).RuleResult();

				gate /= oddsRule;

				if (gate > 0x0FFFFFFF)
				{
					gate2 = 0x10000000; // > 100%
				}
				else
				{
					gate2 = gate;
				}

				if (_Request.WeaponData.WeaponType == WEAPON_TYPE.BIG_OCTOPUS_BOMB)
				{
					gate2 = 0x10000000; // > 100% 
				}

				if (Random.Instance.NextInt(0, 0x10000000) >= gate2)
				{

					return _Miss(fishData, _Request.WeaponData);
				}

				var win = fishData.FishOdds * _Request.WeaponData.WepBet * oddsRule;

				new DiedHandleRule(_StageVisitor, _PlayerRecord, win).Run();
				
				return _Die(fishData, _Request.WeaponData);
			}
			return new HitResponse();
		}

		private HitResponse _Die(RequsetFishData fish_data, RequestWeaponData weapon_data)
		{
			var bufferData = _StageVisitor.NowData.FindBuffer(_StageVisitor.NowBlock, StageBuffer.BUFFER_TYPE.NORMAL);
			return new HitResponse
			{
				WepID = weapon_data.WepID, 
				FishID = fish_data.FishID, 
				DieResult = FISH_DETERMINATION.DEATH, 
				SpecialWeaponType = _PlayerRecord.NowSpecialWeaponData.WeaponType, 
				WUp = new OddsRuler(fish_data, bufferData).RuleResult()
			};
		}

		private HitResponse _Miss(RequsetFishData fish_data, RequestWeaponData weapon_data)
		{
			var bufferData = _StageVisitor.NowData.FindBuffer(_StageVisitor.NowBlock, StageBuffer.BUFFER_TYPE.NORMAL);
			return new HitResponse
			{
				WepID = weapon_data.WepID, 
				FishID = fish_data.FishID, 
				DieResult = FISH_DETERMINATION.SURVIVAL, 
				SpecialWeaponType = _PlayerRecord.NowSpecialWeaponData.WeaponType, 
				WUp = new OddsRuler(fish_data, bufferData).RuleResult()
			};
		}
	}
}