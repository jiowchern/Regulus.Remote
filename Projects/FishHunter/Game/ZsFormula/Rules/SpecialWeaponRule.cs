// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialWeaponRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   特殊武器的處理
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

using Random = Regulus.Utility.Random;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	/// <summary>
	///     特殊武器的處理
	/// </summary>
	public class SpecialWeaponRule
	{
		private readonly StageDataVisit _StageDataVisit;

		public Func<int> Win { get; private set; }

		public SpecialWeaponRule(StageDataVisit stage_data_visit)
		{
			_StageDataVisit = stage_data_visit;
		}

		public void Run(AttackData attack_data)
		{
			var weaponData = new WeaponDataTable().FindWeaponData(attack_data.WeaponData.WeaponType);

			var gate = (int)weaponData.Power; // 特武威力
			var gate2 = 0;

			gate *= 0x0FFFFFFF;

			gate /= attack_data.TotalHitOdds; // 总倍数

			var oddsRule = new OddsRuler(
				attack_data.FishData, 
				_StageDataVisit.FindBufferData(_StageDataVisit.NowUseBlock, StageDataTable.BufferData.BUFFER_TYPE.NORMAL))
				.RuleResult();

			gate /= oddsRule;

			if (gate > 0x0FFFFFFF)
			{
				gate2 = 0x10000000; // > 100%
			}
			else
			{
				gate2 = gate;
			}

			if (attack_data.WeaponData.WeaponType == WeaponDataTable.Data.WEAPON_TYPE.TYPE_106)
			{
				gate2 = 0x10000000; // > 100% 
			}

			var rand = Random.Instance.NextInt(0, 1000);

			if ((rand % 0x10000000) >= gate2)
			{
				return;
			}

			Win = () => attack_data.FishData.Odds * attack_data.GetWeaponBet() * oddsRule;
		}
	}
}