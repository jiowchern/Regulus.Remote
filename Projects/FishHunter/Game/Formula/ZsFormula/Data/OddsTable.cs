
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

	    public List<Data> Datas { get; }

	    public OddsTable()
		{
            Datas = new List<Data>
			{
				new Data(2, 89), 
				new Data(3, 50), 
				new Data(5, 28), 
				new Data(10, 54)
			};
		}
	}
}
