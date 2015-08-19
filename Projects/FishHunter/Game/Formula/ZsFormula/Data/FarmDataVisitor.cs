using System.Collections.Generic;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class FarmDataVisitor
	{
		public IRandom Random { get; set; }

		public FormulaPlayerRecord PlayerRecord { get; private set; }

		public FishFarmData FocusFishFarmData { get; private set; }

		public FarmBuffer.BUFFER_BLOCK FocusBufferBlock { get; set; }

		public List<WEAPON_TYPE> GetItems { get; private set; }

		public FarmDataVisitor(FishFarmData fish_farm_data, FormulaPlayerRecord formula_player_record, IRandom random)
		{
			PlayerRecord = formula_player_record;
			Random = random;
			FocusFishFarmData = fish_farm_data;
			GetItems = new List<WEAPON_TYPE>();
		}
	}
}
