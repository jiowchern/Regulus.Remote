using System.Collections.Generic;

namespace VGame.Project.FishHunter.Common.Data
{
	public class StageRecord
	{
		public int StageId { get; set; }

		public int PlayTotal { get; set; } // 0

		public int WinScore { get; set; }

		public int PlayTimes { get; set; }

		public int WinFrequency { get; set; }

		public int AsnTimes { get; set; }

		public int AsnWin { get; set; }

		public List<SpecialWeaponData> SpecialWeaponDatas { get; private set; }

		public StageRecord(int stage_id)
		{
			StageId = stage_id;
			_CreateSpacialWeaponDatas();
		}

		private void _CreateSpacialWeaponDatas()
		{
			SpecialWeaponDatas = new List<SpecialWeaponData>
			{
				new SpecialWeaponData(weapon_type: WEAPON_TYPE.SUPER_BOMB, sp_id: 0, power: 250), 
				new SpecialWeaponData(WEAPON_TYPE.ELECTRIC_NET, 1, 150 / 15), 
				new SpecialWeaponData(WEAPON_TYPE.SCREEN_BOMB, 3, 80), 
				new SpecialWeaponData(WEAPON_TYPE.THUNDER_BOMB, 4, 150), 
				new SpecialWeaponData(WEAPON_TYPE.FIRE_BOMB, 5, 120 / 15), 
				new SpecialWeaponData(WEAPON_TYPE.DAMAGE_BALL, 6, 200 / 15), 
				new SpecialWeaponData(WEAPON_TYPE.OCTOPUS_BOMB, 7, 200), 
				new SpecialWeaponData(WEAPON_TYPE.BIG_OCTOPUS_BOMB, 8, 10000)
			};
		}
	}
}