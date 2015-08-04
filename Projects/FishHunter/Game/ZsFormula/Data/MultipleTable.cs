// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the MultipleTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Data
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