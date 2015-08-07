// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialWeaponData.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SpecialWeaponData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VGame.Project.FishHunter.Common.Data
{
	public class SpecialWeaponData
	{
		public WEAPON_TYPE WeaponType { get; set; }
		public bool HaveWeapon { get; set; }

		public int SpId { get; set; }

		public float Power { get; set; }

		public int WinFrequency { get; set; }

		public int WinScore { get; set; }

		public SpecialWeaponData(WEAPON_TYPE weapon_type , int sp_id, float power)
		{
			WeaponType = weapon_type;
			SpId = sp_id;
			Power = power;
			HaveWeapon = false;
			WinFrequency = 0;
			WinScore = 0;
		}
	}
}