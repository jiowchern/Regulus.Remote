using System.Collections.Generic;


using Regulus.Utility;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class RandomData
	{
		public enum RULE
		{
			ADJUSTMENT_PLAYER_PHASE,

			CHECK_TREASURE,

			DEATH,

			ODDS
		}

		public List<IRandom> Randoms { get; set; }

		public RULE RandomType { get; set; }

		public int[] RandomValue { get; set; }

		public int RandomCount { get; set; }
	}
}