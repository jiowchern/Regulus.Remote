using System.Collections.Generic;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class OddsTable
	{
		public class Data
		{
			public int Odds { get; set; }

			public int Number { get; set; }

			public Data(int odds, int number)
			{
				Odds = odds;
				Number = number;
			}
		}

		private readonly List<Data> _Datas;

		public OddsTable()
		{
			_Datas = new List<Data>
			{
				new Data(2, 89), 
				new Data(3, 50), 
				new Data(5, 28), 
				new Data(10, 54)
			};
		}

		/// <summary>
		///     不成立要為1 ODDS
		/// </summary>
		/// <param name="rand_number"></param>
		/// <returns></returns>
		public int CheckRule(int rand_number)
		{
			Data tmp = null;

			foreach(var d in _Datas)
			{
				if(rand_number < d.Number)
				{
					tmp = d;
					break;
				}

				rand_number -= d.Number;
			}

			return tmp?.Odds ?? 1;
		}
	}
}
