using System.Collections.Generic;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	/// <summary>
	/// 特武出現率使用
	/// </summary>
	public class SpecialWeaponRateTable
	{
		public class WeaponRate
		{
			public WEAPON_TYPE WeaponType { get; set; }

			public int Rate { get; set; }

			public WeaponRate(WEAPON_TYPE weapon_type, int rate)
			{
				WeaponType = weapon_type;
				Rate = rate;
			}
		}

		public List<WeaponRate> WeaponRates { get; private set; }

		public SpecialWeaponRateTable()
		{
			//ULONG const ulChanceSpecOdds[3] = { 250, 150, 120 }; // 炸弹、电网、免费炮

			WeaponRates = new List<WeaponRate>
			{
				new WeaponRate(WEAPON_TYPE.SUPER_BOMB, 250),
				new WeaponRate(WEAPON_TYPE.ELECTRIC_NET, 150),
				new WeaponRate(WEAPON_TYPE.FREE_POWER, 120),
			};
		}

	}
}