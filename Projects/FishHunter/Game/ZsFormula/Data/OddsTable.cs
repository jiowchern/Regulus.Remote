// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OddsTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   倍數表
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	public class OddsTable
	{
		private readonly List<Data> _Datas;

		public OddsTable(IEnumerable<Data> datas)
		{
			_Datas = datas.ToList();
		}

		public OddsTable()
		{
			var datas = new List<Data>
			{
				new Data(1, 1),
				new Data(2, 2),
				new Data(3, 3),
				new Data(5, 5),
				new Data(10, 10)
			};
		}

		public class Data
		{
			public Data(int key, int number)
			{
				Odds = key;
				Number = number;
			}

			public int Odds { get; set; }

			public int Number { get; set; }
		}

		public IEnumerable<Data> Get()
		{
			return _Datas;
		}
	}
}