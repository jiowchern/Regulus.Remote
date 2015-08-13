using System.Collections.Generic;


using Regulus.Game;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class NatureBufferChancesTable : RangeChancesTable<int>
	{
		public NatureBufferChancesTable(Data[] datas) : base(datas)
		{
		}

		public NatureBufferChancesTable()
		{
			_Datas = new[]
			{
				new Data(-3, -500), 
				new Data(-2, -150), 
				new Data(-1, -100), 
				new Data(0, -50), 
				new Data(1, -30), 
				new Data(2, 0), 
				new Data(3, 20), 
				new Data(4, 50), 
				new Data(5, 100), 
				new Data(6, 150)
			};
		}

		public IEnumerable<Data> Get()
		{
			return _Datas;
		}
	}
}
