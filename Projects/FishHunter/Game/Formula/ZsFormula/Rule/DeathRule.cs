
using System.Collections.Generic;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     // 是否死亡的判断
	/// </summary>
	public class DeathRule
	{
		private readonly HitRequest _Request;

		private readonly StageDataVisitor _StageVisitor;

		private readonly List<HitResponse> _HitResponses;

		public DeathRule(StageDataVisitor stage_visitor, HitRequest request)
		{
			_StageVisitor = stage_visitor;
			_Request = request;
			_HitResponses = new List<HitResponse>();
		}

		public HitResponse[] Run()
		{
			int hitSequence = 0;
			foreach(var fishData in _Request.FishDatas)
			{
				var data = new WeaponPowerTable().WeaponPowers.Find(x => x.WeaponType == _Request.WeaponData.WeaponType);
				var win = data != null ? _SpecialWeapon(fishData) : _NomralWeapon(fishData, hitSequence);

				_DieHandle(win, fishData);
				++hitSequence;
			}

			return _HitResponses.ToArray();
		}

		private int _SpecialWeapon(RequsetFishData fish_data)
		{
			long dieRate = new WeaponPowerTable().WeaponPowers.Find(x => x.WeaponType == _Request.WeaponData.WeaponType).Power;

			// 特武威力
			long gate2;

			dieRate *= 0x0FFFFFFF;

			dieRate /= _Request.WeaponData.TotalHitOdds; // 总倍数

			var bufferData = _StageVisitor.FocusFishFarmData.FindBuffer(
				_StageVisitor.FocusBufferBlock, 
				FarmBuffer.BUFFER_TYPE.NORMAL);

			var oddsRule = new OddsRuler(_StageVisitor, fish_data, bufferData).RuleResult();

			dieRate /= oddsRule;

			if(dieRate > 0x0FFFFFFF)
			{
				gate2 = 0x10000000; // > 100%
			}
			else
			{
				gate2 = dieRate;
			}

			if(_Request.WeaponData.WeaponType == WEAPON_TYPE.BIG_OCTOPUS_BOMB)
			{
				gate2 = 0x10000000; // > 100% 
			}

			if (_StageVisitor.Random.NextInt(0, 0x10000000) >= gate2)
			{
				_Miss(fish_data, _Request.WeaponData);
				return 0;
			}

			var bet = _Request.WeaponData.WepBet * _Request.WeaponData.WepOdds;
			var win = fish_data.FishOdds * bet * oddsRule;

			return win;
		}

		private int _NomralWeapon(RequsetFishData fish_data, int hit_sequence)
		{
			var bufferData = _StageVisitor.FocusFishFarmData.FindBuffer(
				_StageVisitor.FocusBufferBlock, 
				FarmBuffer.BUFFER_TYPE.SPEC);

			long dieRate = _StageVisitor.FocusFishFarmData.GameRate - 10;

			dieRate -= bufferData.Rate;

			dieRate += bufferData.BufferTempValue.HiLoRate;

			if(_StageVisitor.PlayerRecord.Status != 0)
			{
				dieRate += 200; // 提高20%
			}

			if(_Request.WeaponData.WeaponType == WEAPON_TYPE.FREE_POWER)
			{
				// 特武 免费炮
				dieRate /= 2;
			}

			if(dieRate < 0)
			{
				dieRate = 0;
			}

			dieRate *= 0x0FFFFFFF; // 自然死亡率

			dieRate *= _Request.WeaponData.WepBet; // 子弹威力

			// TODO 公式有疑問
			dieRate *= new FishHitAllocateTable().GetAllocateData(_Request.WeaponData.TotalHits, hit_sequence);

			dieRate /= 1000;

			dieRate /= fish_data.FishOdds; // 鱼的倍数

			var oddsRule = new OddsRuler(_StageVisitor, fish_data, bufferData).RuleResult();

			dieRate /= oddsRule; // 翻倍

			dieRate /= 1000; // 死亡率换算回实际百分比

			if(dieRate > 0x0FFFFFFF)
			{
				dieRate = 0x10000000; // > 100%
			}

			if (_StageVisitor.Random.NextInt(0, 0x10000000) >= dieRate)
			{
				_Miss(fish_data, _Request.WeaponData);
				return 0;
			}

			var bet = _Request.WeaponData.WepBet * _Request.WeaponData.WepOdds;
			var win = fish_data.FishOdds * bet * oddsRule;

			return win;
		}

		private void _DieHandle(int win, RequsetFishData fish_data)
		{
			new SaveScoreHistory(_StageVisitor, win).Run();
			new SaveDeathFishHistory(_StageVisitor, fish_data).Run();
			new GetSpecialWeaponRule(_StageVisitor, fish_data).Run();

			_Die(fish_data, _Request.WeaponData);
		}

		private void _Die(RequsetFishData fish_data, RequestWeaponData weapon_data)
		{
			var bufferData = _StageVisitor.FocusFishFarmData.FindBuffer(
				_StageVisitor.FocusBufferBlock, 
				FarmBuffer.BUFFER_TYPE.NORMAL);

			_HitResponses.Add(
				new HitResponse
				{
					WepId = weapon_data.WepId, 
					FishId = fish_data.FishId, 
					DieResult = FISH_DETERMINATION.DEATH, 
					FeedbackWeaponType = _StageVisitor.GetItems.ToArray(),
					WUp = new OddsRuler(_StageVisitor, fish_data, bufferData).RuleResult()
				});
		}

		private void _Miss(RequsetFishData fish_data, RequestWeaponData weapon_data)
		{
			_HitResponses.Add(
				new HitResponse
				{
					WepId = weapon_data.WepId, 
					FishId = fish_data.FishId, 
					DieResult = FISH_DETERMINATION.SURVIVAL, 
					FeedbackWeaponType = new[]
					{
						WEAPON_TYPE.INVALID
					}, 
					WUp = 0
				});
		}
	}
}
