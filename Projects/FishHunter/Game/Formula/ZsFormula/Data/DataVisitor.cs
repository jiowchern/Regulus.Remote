using System;
using System.Collections.Generic;
using System.Linq;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class DataVisitor
	{
		public FormulaPlayerRecord PlayerRecord { get; private set; }

		public FishFarmData Farm { get; private set; }

		public FarmDataRoot.BlockNode.BLOCK_NAME FocusBlockName { get; set; }

		public List<WEAPON_TYPE> GotTreasures { get; set; }

		public List<RandomData> RandomDatas { get; private set; }

		public DataVisitor(FishFarmData fish_farm, FormulaPlayerRecord formula_player_record, List<RandomData> random)
		{
			RandomDatas = random;

			Farm = fish_farm;
			PlayerRecord = formula_player_record;

			GotTreasures = new List<WEAPON_TYPE>();
		}

		public IRandom FindIRandom(RandomData.RULE rule_type, int index)
		{
			return RandomDatas.Find(x => x.RandomType == rule_type).Randoms.ElementAt(index);
		}
	}
}
