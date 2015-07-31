// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BetChancesTable.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the BetChancesTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Linq;

using Regulus.Game;

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public class BetChancesTable : RangeChancesTable<int>
	{
		public int MaxBet { private get; set; }

		public int PlayerBet { private get; set; }

		private float Precent
		{
			get { return PlayerBet / (float)MaxBet; }
		}

		public BetChancesTable(Data[] datas) : base(datas)
		{
		}

		public int GetDiceKey()
		{
			return Dice(Precent);
		}

		public int GetCount()
		{
			return _Datas.Count();
		}
	}
}