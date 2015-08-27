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

			public int HitTotal { get; private set; }

			public object Hit { get; set; }

			public Data(int hit_total, int hit1, int hit2, int hit3, int hit4)
			{
				HitTotal = hit_total;
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


		public int GetAllocateData(int total_hits, int hit_sequence)
		{
			switch(hit_sequence)
			{
			    case 0:
			        return (from d in _Datas where d.HitTotal == total_hits select d.Hit1).DefaultIfEmpty<int>(_Datas.Last().Hit1).First();
			    case 1:
			        return (from d in _Datas where d.HitTotal == total_hits select d.Hit2).DefaultIfEmpty<int>(_Datas.Last().Hit2).First();
			    case 2:
			        return (from d in _Datas where d.HitTotal == total_hits select d.Hit3).DefaultIfEmpty<int>(_Datas.Last().Hit3).First();
			    case 3:
			        return (from d in _Datas where d.HitTotal == total_hits select d.Hit4).DefaultIfEmpty<int>(_Datas.Last().Hit4).First();
			}

		    return 0;
		}
	}
}
