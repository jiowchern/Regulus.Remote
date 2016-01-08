using System.Collections.Generic;
using System.Linq;

using VGame.Project.FishHunter.Common.Data;

using Random = Regulus.Utility.Random;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds
{
	public class WhaleSliver : IFloatingCalculator
	{
		private readonly List<OddsData> _OddsDatas;

		public WhaleSliver()
		{
			_OddsDatas = new List<OddsData>
			{
				new OddsData
				{
					Odds = 100, 
					Rate = 0
				}, 
				new OddsData
				{
					Odds = 150, 
					Rate = 1
				}, 
				new OddsData
				{
					Odds = 200, 
					Rate = 2
				}, 
				new OddsData
				{
					Odds = 250, 
					Rate = 3
				}, 
				new OddsData
				{
					Odds = 300, 
					Rate = 4
				}
			};
		}

		void IFloatingCalculator.Calculate(RequsetFishData[] fish_data)
		{
			var whales = fish_data.Where(x => x.FishType == FISH_TYPE.WHALE_SLIVER);

			foreach(var whale in whales)
			{
				var random = Random.Instance.NextInt(0, 6);
				whale.FishOdds = _OddsDatas.Find(x => x.Rate == random)
											.Odds;
			}
		}
	}
}
