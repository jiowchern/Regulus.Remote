// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   倍數表定義
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public class MultipleTable
	{
		public Dictionary<int, Data> Datas { get; private set; }

		public MultipleTable(IEnumerable<Data> datas)
		{
			Datas = datas.ToDictionary(x => x.Multiple);
		}

		public class Data
		{
			public int Multiple { get; set; }

			public int Value { get; set; }
		}

		public Data Find(int multiple)
		{
			return Datas[multiple];
		}

		public int Count()
		{
			return Datas.Count;
		}
	}
}