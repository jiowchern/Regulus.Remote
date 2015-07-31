// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NatureBufferChancesTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the NatureBufferChancesTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;

using Regulus.Game;
using Regulus.Utility;

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public class NatureBufferChancesTable : RangeChancesTable<int>
	{
		public Dictionary<int, Data> Datas { get; private set; }

		public NatureBufferChancesTable(Data[] datas) : base(datas)
		{
		}

		public Data Find(int id)
		{
			Datas = _Datas.ToDictionary(x => x.Key);
			return Datas[id];
		}

		public int FindValue(int id)
		{
			var datas = _Datas.ToDictionary(x => x.Key);
			return (int)datas[id].Value;
		}

		public int CheckRate(int rate)
		{
			{
				// if (fish_data.Odds < 30)
				return 0;
			}

			if (rate <= _Datas[0].Value)
			{
				var random = Random.Instance.NextInt(0, 1000);
				if (random < 750)
				{
					return 1;
				}
			}

			if (rate <= _Datas[1].Value)
			{
				var random = Random.Instance.NextInt(0, 1000);
				if (random < 500)
				{
					return 1;
				}
			}

			if (rate <= _Datas[2].Value)
			{
				var random = Random.Instance.NextInt(0, 1000);
				if (random < 250)
				{
					return 1;
				}
			}

			if (rate <= _Datas[3].Value)
			{
				var random = Random.Instance.NextInt(0, 1000);
				if (random < 100)
				{
					return 1;
				}
			}

			return 0;
		}
	}
}