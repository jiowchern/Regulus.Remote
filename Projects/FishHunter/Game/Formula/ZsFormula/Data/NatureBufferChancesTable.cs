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
				new Data(-2, -400), 
				new Data(-1, -300), 
				new Data(0, -200), 
				new Data(1, -100), 
				new Data(2, 0), 
				new Data(3, 100), 
				new Data(4, 200), 
				new Data(5, 300), 
				new Data(6, 400), 
				new Data(7, 500)
			};
		}

		public IEnumerable<Data> Get()
		{
			return _Datas;
		}
	}
}
