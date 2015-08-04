// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponTypeRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the WeaponTypeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	public class WeaponTypeRule
	{
		private readonly StageDataVisit _StageDataVisit;

		public WeaponTypeRule(StageDataVisit stage_data_visit)
		{
			_StageDataVisit = stage_data_visit;
		}

		public void Run(Func<int> win_func, AttackData attack_data, Player.Data player_data)
		{
			if (win_func == null)
			{
				return;
			}

			var win = win_func();

			if (attack_data.WeaponData.WeaponType != WeaponDataTable.Data.WEAPON_TYPE.FREE_4)
			{
				return;
			}

			var playerWeaponData = player_data.RecodeData.SpecialWeaponDatas.Find(x => x.IsUsed == false);

			playerWeaponData.WinScore += win;

			var stageWeaponData = _StageDataVisit.NowUseData.RecodeData.SpecialWeaponDatas.Find(x => x.IsUsed == false);
			stageWeaponData.WinScore += win;
			
		}
	}
}