// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NatureBufferChancesTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   ¦ÛµMbufferªí
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;


using Regulus.Game;

namespace VGame.Project.FishHunter.ZsFormula.Data
{
	public class NatureBufferChancesTable : RangeChancesTable<int>
	{
		public NatureBufferChancesTable(Data[] datas) : base(datas)
		{
		}

		public NatureBufferChancesTable() : base()
		{
			_Datas = new Data[]
			{
				new ChancesTable<int>.Data(-3, -500),
				new ChancesTable<int>.Data(-2, -150),
				new ChancesTable<int>.Data(-1, -100),
				new ChancesTable<int>.Data(0, -50),
				new ChancesTable<int>.Data(1, -30),
				new ChancesTable<int>.Data(2, 0),
				new ChancesTable<int>.Data(3, 20),
				new ChancesTable<int>.Data(4, 50),
				new ChancesTable<int>.Data(5, 100),
				new ChancesTable<int>.Data(6, 150),
			};
		}

		public IEnumerable<Data> Get()
		{
			return _Datas;
		}
	}
}