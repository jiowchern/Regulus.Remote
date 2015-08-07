// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FishHitAllocateTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the FishHitAllocateTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	public class FishHitAllocateTable
	{
		private readonly List<Data> _Datas;

		public FishHitAllocateTable(IEnumerable<Data> datas)
		{
			_Datas = datas.ToList();
		}

		public FishHitAllocateTable()
		{
			_Datas = new List<Data>
			{
				new Data(1, 1000, 0, 0, 0),
				new Data(2, 800, 0, 0, 0),
				new Data(3, 750, 0, 0, 0),
				new Data(4, 7000, 0, 0, 0)
			};
		}

		public class Data
		{
			public Data(int hit_number, int hit1, int hit2, int hit3, int hit4)
			{
				HitNumber = hit_number;
				Hit1 = hit1;
				Hit2 = hit2;
				Hit3 = hit3;
				Hit4 = hit4;
			}

			public int Hit1 { get; private set; }

			public int Hit2 { get; private set; }

			public int Hit3 { get; private set; }

			public int Hit4 { get; private set; }

			public int HitNumber { get; private set; }
		}

		public Data GetAllocateData(int total_hits)
		{
			var dictionary = _Datas.ToDictionary(x => x.HitNumber);

			switch (total_hits)
			{
				case 1:
					return dictionary[1];
				case 2:
					return dictionary[2];
				case 3:
					return dictionary[3];
				case 4:
					return dictionary[4];
				default:
					return dictionary[4];
			}
		}
	}
}