// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialWeaponData.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SpecialWeaponData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VGame.Project.FishHunter.Common.Datas.FishStage
{
	public class SpecialWeaponData
	{
		public bool IsUsed { get; set; }

		public int SpId { get; set; }

		public int WinFrequency { get; set; } // 6

		public int WinScore { get; set; }

		public SpecialWeaponData(int sp_id)
		{
			SpId = sp_id;
			IsUsed = false;
			WinFrequency = 0;
			WinScore = 0;
		}
	}
}