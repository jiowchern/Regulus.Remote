using System.Collections.Generic;

using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class TreasureKind
	{
		public enum KIND
		{
			RANDOM, 

			CERTAIN, 

			COUNT
		}

		public KIND Kind;

		public List<WEAPON_TYPE> Treasures { get; set; }
	}
}
