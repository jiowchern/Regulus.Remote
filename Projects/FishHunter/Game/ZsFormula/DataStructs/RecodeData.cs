// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageRecodeData.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the RecodeData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public class RecodeData
	{
		public class SpecialWeaponData
		{
			public SpecialWeaponData(int sp_id)
			{
				SpId = sp_id;
				IsUsed = false;
				WinFrequency = 0;
				WinScore = 0;
			}

			public bool IsUsed { get; set; }

			public int SpId { get; set; }

			public int WinFrequency { get; set; } // 6

			public int WinScore { get; set; }
		}

		public RecodeData()
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
				new SpecialWeaponData(9),
			};
		}

		public int PlayTotal { get; set; } // 0

		public int WinScore { get; set; }

		public int PlayTimes { get; set; }

		public int WinFrequency { get; set; }

		public int AsnTimes { get; set; }

		public int AsnWin { get; set; }
		
		public List<SpecialWeaponData> SpecialWeaponDatas { get; private set; }
	}
}