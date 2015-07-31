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

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public class FishHitAllocateTable
	{
		private readonly Dictionary<int, Data> _Datas;

		public FishHitAllocateTable(IEnumerable<Data> datas)
		{
			_Datas = datas.ToDictionary(x => x.HitNumber);
		}

		public class Data
		{
			public int Hit1 { get; private set; }

			public int Hit2 { get; private set; }

			public int Hit3 { get; private set; }

			public int Hit4 { get; private set; }

			public int HitNumber { get; private set; }
		}

		public Data GetAllocateData(int total_hits)
		{
			switch (total_hits)
			{
				case 1:
					return _Datas[1];
				case 2:
					return _Datas[2];
				case 3:
					return _Datas[3];
				case 4:
					return _Datas[4];
				default:
					return _Datas[4];
			}
		}
	}
}