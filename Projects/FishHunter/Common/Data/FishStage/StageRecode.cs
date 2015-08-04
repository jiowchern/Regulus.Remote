// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageRecode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StageRecode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace VGame.Project.FishHunter.Common.Datas.FishStage
{
	public class StageRecode
	{
		public int PlayTotal { get; set; } // 0

		public int WinScore { get; set; }

		public int PlayTimes { get; set; }

		public int WinFrequency { get; set; }

		public int AsnTimes { get; set; }

		public int AsnWin { get; set; }

		public List<SpecialWeaponData> SpecialWeaponDatas { get; private set; }

		public StageRecode()
		{
			SpecialWeaponDatas = new List<SpecialWeaponData>
			{
				new SpecialWeaponData(0), 
				new SpecialWeaponData(1), 
				new SpecialWeaponData(2), 
				new SpecialWeaponData(3), 
				new SpecialWeaponData(4), 
				new SpecialWeaponData(5), 
				new SpecialWeaponData(6), 
				new SpecialWeaponData(7), 
				new SpecialWeaponData(8), 
				new SpecialWeaponData(9)
			};
		}

		
	}
}