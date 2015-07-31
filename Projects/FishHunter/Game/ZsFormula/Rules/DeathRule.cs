// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeathRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   是否死亡的判断
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

using Random = Regulus.Utility.Random;

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	/// <summary>
	/// // 是否死亡的判断
	/// </summary>
	public class DeathRule
	{
		private readonly FishHitAllocateTable _FishHitAllocateTable;

		private readonly StageDataVisit _StageDataVisit;

		private Player.Data _PlayerData;

		public Func<int> Win { get; private set; }

		public DeathRule(StageDataVisit stage_data_visit)
		{
			_StageDataVisit = stage_data_visit;

			_FishHitAllocateTable = new FishHitAllocateTable(null);
		}

		/// <param name="attack_data">
		/// The attack_data.
		/// </param>
		public void Run(AttackData attack_data)
		{
			var fireRate = _StageDataVisit.NowUseData.GameRate - 10;
			var bufferData = _StageDataVisit.FindBufferData(
				_StageDataVisit.NowUseBlock, 
				StageDataTable.BufferData.BUFFER_TYPE.SPEC);

			fireRate -= (int)bufferData.Rate;

			fireRate += bufferData.BufferTempValue.HiLoRate;

			if (_PlayerData.Status != 0)
			{
				fireRate += 200; // 提高20%
			}

			if (attack_data.WeaponData.WeaponType == WeaponDataTable.Data.WEAPON_TYPE.FREE_NORMAL_4)
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
			gate *= attack_data.WeaponData.WeaponBet; // 子弹威力

			gate *= _FishHitAllocateTable.GetAllocateData(attack_data.AttCount).HitNumber;

			gate /= 1000;

			gate /= attack_data.FishData.Odds; // 鱼的倍数

			var oddsRule = new OddsRuler(attack_data.FishData, bufferData).RuleResult();
			gate /= oddsRule; // 翻倍
			gate /= 1000; // 死亡率换算回实际百分比

			if (gate > 0x0FFFFFFF)
			{
				gate = 0x10000000; // > 100%
			}

			var rand = Random.Instance.NextInt(0, 1000) % 0x10000000;
			if (rand < gate)
			{
				var win = attack_data.FishData.Odds * attack_data.GetWeaponBet() * oddsRule;

				Win = () => win;
				Win.Invoke();
			}
		}
	}
}