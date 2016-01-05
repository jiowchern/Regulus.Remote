using Regulus.Game;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class BetChancesTable : RangeChancesTable<int>
	{
		public int MaxBet { private get; set; }

		public int PlayerBet { private get; set; }

		private float _Precent => PlayerBet / (float)MaxBet;

		public BetChancesTable(Data[] datas) : base(datas)
		{
		}

		public int GetDiceKey()
		{
			return Dice(_Precent);
		}
	}
}
