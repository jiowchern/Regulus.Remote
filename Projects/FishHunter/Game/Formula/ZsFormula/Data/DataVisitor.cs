using System.Collections.Generic;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class DataVisitor
	{
		public IRandom Random { get; set; }

		public FormulaPlayerRecord PlayerRecord { get; private set; }

		public FishFarmData Farm { get; private set; }

		public FarmBuffer.BUFFER_BLOCK FocusBufferBlock { get; set; }

		public List<WEAPON_TYPE> GotTreasures { get; private set; }


        public DataVisitor(FishFarmData fish_farm, FormulaPlayerRecord formula_player_record, IRandom random)
        {
            Random = random;
			Farm = fish_farm;
			GotTreasures = new List<WEAPON_TYPE>();
        }
	}
}
