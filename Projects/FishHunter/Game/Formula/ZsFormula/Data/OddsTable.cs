using System.Collections.Generic;
using System.Linq;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class OddsTable
	{
		public class Data
		{
			public int Odds { get; set; }

			public int Number { get; set; }

			public Data(int key, int number)
			{
				Odds = key;
				Number = number;
			}
		}

		private readonly List<Data> _Datas;

		public OddsTable()
		{
            _Datas = new List<Data>
			{
				new Data(1, 1), 
				new Data(2, 2), 
				new Data(3, 3), 
				new Data(5, 5), 
				new Data(10, 10)
			};
		}

		public IEnumerable<Data> Get()
		{
			return _Datas;
		}
	}
}
