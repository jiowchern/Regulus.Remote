using System.Collections.Generic;

using Regulus.Utility;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class CreateRandom
	{
		public static List<RandomData> GetRandoms()
		{
			var rs = new List<RandomData>
			{
				_CreateAdjustPlayerPhaseRandom(), 
				_CreateTreasureRandom(), 
				_CreateDeathRandom(), 
				_CreateOddsRandom()
			};

			return rs;
		}

		private static RandomData _CreateAdjustPlayerPhaseRandom()
		{
			var data = new RandomData
			{
				RandomType = RandomData.RULE.ADJUSTMENT_PLAYER_PHASE, 
				Randoms = new List<IRandom>()
			};

			data.Randoms.Add(Random.Instance);
			data.Randoms.Add(Random.Instance);
			return data;
		}

		private static RandomData _CreateTreasureRandom()
		{
			var data = new RandomData
			{
				RandomType = RandomData.RULE.CHECK_TREASURE, 
				Randoms = new List<IRandom>()
			};

			data.Randoms.Add(Random.Instance);
			data.Randoms.Add(Random.Instance);

			return data;
		}

		private static RandomData _CreateDeathRandom()
		{
			var data = new RandomData
			{
				RandomType = RandomData.RULE.DEATH, 
				Randoms = new List<IRandom>()
			};

			data.Randoms.Add(Random.Instance);
			data.Randoms.Add(Random.Instance);

			return data;
		}

		private static RandomData _CreateOddsRandom()
		{
			var data = new RandomData
			{
				RandomType = RandomData.RULE.ODDS, 
				Randoms = new List<IRandom>()
			};

			data.Randoms.Add(Random.Instance);

			data.Randoms.Add(Random.Instance);

			data.Randoms.Add(Random.Instance);

			data.Randoms.Add(Random.Instance);

			data.Randoms.Add(Random.Instance);

			return data;
		}
	}
}
