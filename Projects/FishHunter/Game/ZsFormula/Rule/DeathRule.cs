// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeathRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   是否死亡的判断
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     // 是否死亡的判断
	/// </summary>
	public class DeathRule
	{
		private readonly FishStageVisitor _StageVisitor;

		private readonly HitRequest _Request;

		private readonly PlayerRecord _PlayerRecord;

		public DeathRule(FishStageVisitor stage_visitor, HitRequest request, PlayerRecord player_record)
		{
			_StageVisitor = stage_visitor;
			_Request = request;
			_PlayerRecord = player_record;
		}


		public HitResponse Run()
		{
			foreach (var fishData in _Request.FishDatas)
			{
				var fireRate = _StageVisitor.NowData.GameRate - 10;
				var bufferData = _StageVisitor.NowData.FindBuffer(
					_StageVisitor.NowBlock,
					StageBuffer.BUFFER_TYPE.SPEC);

				fireRate -= (int)bufferData.Rate;

				fireRate += bufferData.BufferTempValue.HiLoRate;

				if (_PlayerRecord.Status != 0)
				{
					fireRate += 200; // 提高20%
				}

				if (_Request.WeaponData.WeaponType == WEAPON_TYPE.FREE_POWER)
				{
					// 特武 免费炮
					fireRate /= 2;
				}

				if (fireRate < 0)
				{
					fireRate = 0;
				}

				var gate = fireRate; // 自然死亡率

				gate *= 0x0FFFFFFF;

				gate *= _Request.WeaponData.WepBet; // 子弹威力

				gate *= new FishHitAllocateTable().GetAllocateData(_Request.WeaponData.TotalHits).HitNumber;

				gate /= 1000;

				gate /= _Request.WeaponData.TotalHitOdds; // 鱼的倍数

				var oddsRule = new OddsRuler(fishData, bufferData).RuleResult();

				gate /= oddsRule; // 翻倍

				gate /= 1000; // 死亡率换算回实际百分比

				if (gate > 0x0FFFFFFF)
				{
					gate = 0x10000000; // > 100%
				}

				if (Regulus.Utility.Random.Instance.NextInt(0, 0x10000000) >= gate)
				{
					return _Miss(fishData, _Request.WeaponData);
				}

				var win = fishData.FishOdds * _Request.WeaponData.WepBet * oddsRule;

				new DiedHandleRule(_StageVisitor, _PlayerRecord, win).Run();
				new FishTypeRule(_StageVisitor, fishData, _PlayerRecord).Run();
				new WeaponTypeRule(_StageVisitor, _Request.WeaponData, _PlayerRecord, win).Run();
				new SpecialItemRule(_StageVisitor, _PlayerRecord).Run();
				
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
				WUp = new OddsRuler(fish_data, bufferData).RuleResult(),
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
				WUp = new OddsRuler(fish_data, bufferData).RuleResult(),
			};
		}
	}
}