// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageBufferChecker.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the StageBufferChecker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using Regulus.Utility;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace VGame.Project.FishHunter.ZsFormula
{
	public class StageBufferChecker
	{
		private readonly MultipleTable _MultipleTable;

		private readonly NatureBufferChancesTable _NatureBufferChancesTable;

		private FishDataTable.Data _FishData;

		public StageBufferChecker(StageDataTable.BufferData buffer_data)
		{
			// todo:
			_NatureBufferChancesTable = new NatureBufferChancesTable(null);
			_MultipleTable = new MultipleTable(null);
			var rate = buffer_data.Rate;
			CheckNatureBuffer(rate);
			CheckMultipleRule();
		}

		private int CheckMultipleRule()
		{
			var rand = Random.Instance.NextInt(0, 1000);
			var wup = 1;
			var i = 0;

			for (i = 0; i < _MultipleTable.Count(); ++i)
			{
				var value = _MultipleTable.Find(i).Value;
				if (rand < value)
				{
					break;
				}

				rand -= value;
			}

			if (i < 4)
			{
				wup = _MultipleTable.Find(i).Value;
			}

			if (_FishData.Odds < 50 && wup == 10)
			{
				wup = 1;
			}

			return wup;
		}

		public int CheckNatureBuffer(long rate)
		{
			if (_FishData.Odds < 30)
			{
				return 0;
			}

			if (rate <= _NatureBufferChancesTable.Find(0).Value)
			{
				var rand = Random.Instance.NextInt(0, 1000);
				if (rand < 750)
				{
					return 1;
				}
			}
			else if (rate <= _NatureBufferChancesTable.Find(1).Value)
			{
				var rand = Random.Instance.NextInt(0, 1000);
				if (rand < 500)
				{
					return 1;
				}
			}
			else if (rate <= _NatureBufferChancesTable.Find(2).Value)
			{
				var rand = Random.Instance.NextInt(0, 1000);
				if (rand < 250)
				{
					return 1;
				}
			}
			else if (rate <= _NatureBufferChancesTable.Find(3).Value)
			{
				var rand = Random.Instance.NextInt(0, 1000);
				if (rand < 100)
				{
					return 1;
				}
			}

			return 0;
		}
	}
}