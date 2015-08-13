// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FishHitAllocateTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   同時擊中隻數的分配表
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	/// <summary>
	/// 同時擊中隻數的分配表
	/// </summary>
	public class FishHitAllocateTable
	{
		public class Data
		{
			public int Hit1 { get; private set; }

			public int Hit2 { get; private set; }

			public int Hit3 { get; private set; }

			public int Hit4 { get; private set; }

			public int HitNumber { get; private set; }

			public Data(int hit_number, int hit1, int hit2, int hit3, int hit4)
			{
				HitNumber = hit_number;
				Hit1 = hit1;
				Hit2 = hit2;
				Hit3 = hit3;
				Hit4 = hit4;
			}
		}

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
				new Data(2, 800, 200, 0, 0), 
				new Data(3, 750, 150, 100, 0), 
				new Data(4, 700, 150, 100, 50)
			};
		}

		/// <summary>
		///同時擊中4隻以上分配一樣
		/// </summary>
		/// <param name="total_hits"></param>
		/// <returns></returns>
		public Data GetAllocateData(int total_hits)
		{
			var dictionary = _Datas.ToDictionary(x => x.HitNumber);

			switch(total_hits)
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
