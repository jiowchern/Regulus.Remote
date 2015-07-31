// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponTypeRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the WeaponTypeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


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

		public void Run(AttackData attack_data, Player.Data player_data)
		{
			if (attack_data.WeaponData.WeaponType == WeaponDataTable.Data.WEAPON_TYPE.FREE_NORMAL_4)
			{
				player_data.Recode.Sp02WinTotal++;
				_StageDataVisit.NowUseData.Recode.Sp02WinTotal++;
			}
		}
	}
}