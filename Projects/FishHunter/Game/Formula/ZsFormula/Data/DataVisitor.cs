using System.Collections.Generic;
using System.Linq;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class DataVisitor
	{
		private readonly List<TreasureKind> _Treasures;

		public FormulaPlayerRecord PlayerRecord { get; private set; }

		public FishFarmData Farm { get; private set; }

		public FarmDataRoot.BlockNode.BLOCK_NAME FocusBlockName { get; set; }

		public DataVisitor(FishFarmData fish_farm, FormulaPlayerRecord formula_player_record)
		{
			Farm = fish_farm;
			PlayerRecord = formula_player_record;

			_Treasures = new List<TreasureKind>
			{
				new TreasureKind
				{
					Kind = TreasureKind.KIND.RANDOM, 
					Treasures = new List<WEAPON_TYPE>()
				}, 
				new TreasureKind
				{
					Kind = TreasureKind.KIND.CERTAIN, 
					Treasures = new List<WEAPON_TYPE>()
				}
			};
		}

		public List<WEAPON_TYPE> GetTreasures(TreasureKind.KIND kind)
		{
			return _Treasures.Find(x => x.Kind == kind)
							.Treasures;
		}

		public IRandom FindIRandom(RandomData.RULE rule_type, int index)
		{
			return CreateRandom.GetRandoms()
								.Find(x => x.RandomType == rule_type)
								.Randoms.ElementAt(index);
		}

		public List<WEAPON_TYPE> GetAllTreasures()
		{
			var treasures = new List<WEAPON_TYPE>();

			foreach(var data in _Treasures)
			{
				treasures.AddRange(data.Treasures);
			}

			

			return treasures;
		}
	}
}
