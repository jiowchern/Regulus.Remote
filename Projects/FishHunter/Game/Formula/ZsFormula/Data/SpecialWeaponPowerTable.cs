using System.Collections.Generic;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class SpecialWeaponPowerTable
	{
		public class WeaponPower
		{
			public WEAPON_TYPE WeaponType { get; set; }

			public int Power { get; set; }

			public WeaponPower(WEAPON_TYPE weapon_type, int power)
			{
				WeaponType = weapon_type;
				Power = power;
			}
		}

		public List<WeaponPower> WeaponPowers { get; private set; }

		public SpecialWeaponPowerTable()
		{
			WeaponPowers = new List<WeaponPower>
			{
				new WeaponPower(WEAPON_TYPE.SUPER_BOMB, 250),
				new WeaponPower(WEAPON_TYPE.ELECTRIC_NET, 150 / 15),
				new WeaponPower(WEAPON_TYPE.FREE_POWER, 1),
				new WeaponPower(WEAPON_TYPE.SCREEN_BOMB, 80),
				new WeaponPower(WEAPON_TYPE.THUNDER_BOMB, 150),
				new WeaponPower(WEAPON_TYPE.FIRE_BOMB, 120 / 15),
				new WeaponPower(WEAPON_TYPE.DAMAGE_BALL, 200 / 15),
				new WeaponPower(WEAPON_TYPE.OCTOPUS_BOMB, 200),
				new WeaponPower(WEAPON_TYPE.BIG_OCTOPUS_BOMB, 10000)
			};
		}

	}
}