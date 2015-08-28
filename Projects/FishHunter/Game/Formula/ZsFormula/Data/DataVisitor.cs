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

		public FarmBuffer.BUFFER_BLOCK FocusBufferBlock { get; set; }

		public List<WEAPON_TYPE> GotTreasures { get; private set; }

		public List<RandomData> RandomDatas { get; }


		public IRandom FindIRandom(RandomData.RULE rule_type, int index)
		{
			return RandomDatas.Find(x => x.RandomType == rule_type).Randoms.ElementAt(index);
		}
		public DataVisitor(FishFarmData fish_farm, FormulaPlayerRecord formula_player_record, List<RandomData> random)
		{
			Farm = fish_farm;
			PlayerRecord = formula_player_record;

			GotTreasures = new List<WEAPON_TYPE>();

			RandomDatas = random;

			//			RandomDatas = new List<RandomData>
			//			{
			//				new RandomData
			//				{
			//					RandomType = RandomData.RULE.ADJUSTMENT_PLAYER_PHASE,
			//					RandomValue = new[]
			//					{
			//						random.NextInt(0, 1000)
			//					}
			//				},
			//				new RandomData
			//				{
			//					RandomType = RandomData.RULE.CHECK_TREASURE,
			//					RandomValue = new[]
			//					{
			//						random.NextInt(0, 0x10000000),
			//						random.NextInt(0, 3)
			//					}
			//				},
			//
			//				new RandomData
			//				{
			//					RandomType = RandomData.RULE.DEATH,
			//					RandomValue = new[]
			//					{
			//						random.NextInt(0, 0x10000000),
			//						random.NextInt(0, 0x10000000),
			//					}
			//				},
			//				new RandomData
			//				{
			//					RandomType = RandomData.RULE.ODDS,
			//					RandomValue = new[]
			//					{
			//						random.NextInt(0, 1000),
			//						random.NextInt(0, 1000),
			//						random.NextInt(0, 1000),
			//						random.NextInt(0, 1000),
			//						random.NextInt(0, 1000),
			//					}
			//				}
			//			};
		}
	}
}
