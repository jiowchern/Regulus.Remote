
using System.Collections.Generic;
using System.Linq;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     // 是否死亡的判断
	/// </summary>
	public class DeathRule
	{
		private readonly DataVisitor _Visitor;

		private readonly List<HitResponse> _HitResponses;

		private readonly HitRequest _Request;

		public DeathRule(DataVisitor visitor, HitRequest request)
		{
			_Visitor = visitor;
			_Request = request;
			_HitResponses = new List<HitResponse>();
		}

		public HitResponse[] Run()
		{
			var hitSequence = 0;

			foreach (var fishData in _Request.FishDatas)
			{
				// 有值代表是特殊武器
				var specialWeaponPower = new SpecialWeaponPowerTable().WeaponPowers.Find(
					x => x.WeaponType == _Request.WeaponData.WeaponType);

				if (specialWeaponPower != null)
				{
					_SpecialWeapon(fishData);
				}
				else
				{
					_NomralWeapon(fishData, hitSequence);
				}

				++hitSequence;
			}

			return _HitResponses.ToArray();
		}

		private void _SpecialWeapon(RequsetFishData fish_data)
		{
			long dieRate =
				new SpecialWeaponPowerTable().WeaponPowers.Find(x => x.WeaponType == _Request.WeaponData.WeaponType).Power;

			dieRate *= 0x0FFFFFFF;

			dieRate /= _Request.FishDatas.Sum(x => x.FishOdds); // 总倍数

			var bufferData = _Visitor.Farm.FindBuffer(
				_Visitor.FocusBufferBlock, 
				FarmBuffer.BUFFER_TYPE.NORMAL);

			var oddsRule = new OddsRuler(_Visitor, fish_data, bufferData).RuleResult();

			dieRate /= oddsRule;
			

			if(dieRate > 0x0FFFFFFF)
			{
				dieRate = 0x10000000; // > 100%
			}
			
			if(_Request.WeaponData.WeaponType == WEAPON_TYPE.BIG_OCTOPUS_BOMB)
			{
				dieRate = 0x10000000; // > 100% 
			}

			var randomValue = _Visitor.FindIRandom(RandomData.RULE.DEATH, 0).NextInt(0, 0x10000000);

			if(randomValue >= dieRate)
			{
				_Miss(fish_data, _Request.WeaponData, dieRate);
				return;
			}

			var win = fish_data.FishOdds * _Request.WeaponData.GetTotalBet() * oddsRule;

			_DieHandle(win, fish_data, dieRate);
		}

		private void _NomralWeapon(RequsetFishData fish_data, int hit_sequence)
		{
			var bufferData = _Visitor.Farm.FindBuffer(
				_Visitor.FocusBufferBlock, 
				FarmBuffer.BUFFER_TYPE.SPEC);

			long dieRate = _Visitor.Farm.GameRate - 10;

			dieRate -= bufferData.Rate;

			dieRate += bufferData.BufferTempValue.HiLoRate;

			if(_Visitor.PlayerRecord.Status != 0)
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

			dieRate *= _Request.WeaponData.WeaponOdds; // 子弹威力

			dieRate *= new FishHitAllocateTable().GetAllocateData(_Request.WeaponData.TotalHits, hit_sequence);

			dieRate /= 1000;

			dieRate /= fish_data.FishOdds; // 鱼的倍数

			var oddsRule = new OddsRuler(_Visitor, fish_data, bufferData).RuleResult();

			dieRate /= oddsRule; // 翻倍

			dieRate /= 1000; // 死亡率换算回实际百分比

			if(dieRate > 0x0FFFFFFF)
			{
				dieRate = 0x10000000; // > 100%
			}

			var randomValue = _Visitor.FindIRandom(RandomData.RULE.DEATH, 1).NextInt(0, 0x10000000);
			if (randomValue >= dieRate)
			{
				_Miss(fish_data, _Request.WeaponData, dieRate);
				return;
			}

			var bet = _Request.WeaponData.GetTotalBet();
			var win = fish_data.FishOdds * bet * oddsRule;

			_DieHandle(win, fish_data, dieRate);
		}

		private void _DieHandle(int win, RequsetFishData fish_data, long die_rate)
		{
			new SaveDeathFishHistory(_Visitor, fish_data).Run();
			new CheckTreasureRule(_Visitor, fish_data).Run();
			new SaveScoreHistory(_Visitor, win).Run();
			
			_Die(fish_data, _Request.WeaponData, die_rate);
		}

		private void _Die(RequsetFishData fish_data, RequestWeaponData weapon_data, long die_rate)
		{
			var bufferData = _Visitor.Farm.FindBuffer(
				_Visitor.FocusBufferBlock, 
				FarmBuffer.BUFFER_TYPE.NORMAL);

			_HitResponses.Add(
				new HitResponse()
				{
					WepId = weapon_data.BulletId, 
					FishId = fish_data.FishId, 
					DieResult = FISH_DETERMINATION.DEATH, 
					FeedbackWeapons = _Visitor.GotTreasures.ToArray(),
					WeaponBet = weapon_data.WeaponBet,
					DieRate = die_rate,
					OddsResult = new OddsRuler(_Visitor, fish_data, bufferData).RuleResult()
				});
		}

		private void _Miss(RequsetFishData fish_data, RequestWeaponData weapon_data, long die_rate)
		{
			_HitResponses.Add(
				new HitResponse()
				{
					WepId = weapon_data.BulletId, 
					FishId = fish_data.FishId,
					DieResult = FISH_DETERMINATION.SURVIVAL,
					FeedbackWeapons = new[]
					{
						WEAPON_TYPE.INVALID
					},
					WeaponBet = weapon_data.WeaponBet,
					OddsResult = 0,
					DieRate = die_rate
				});
		}
	}
}
