// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttackData.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   §ðÀ»³½ªº¸ê®Æ
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public class AttackData
	{
		public FishDataTable.Data FishData { get; private set; }

		public WeaponDataTable.Data WeaponData { get; private set; }

		public int AttCount { get; set; }

		public int TotalHitOdds { get; set; }

		public AttackData(FishDataTable.Data fish_data, WeaponDataTable.Data weapon_data)
		{
			FishData = fish_data;
			WeaponData = weapon_data;
		}

		public int GetWeaponBet()
		{
			var bet = WeaponData.WeaponOdds * WeaponData.WeaponBet;
			return bet;
		}
	}
}