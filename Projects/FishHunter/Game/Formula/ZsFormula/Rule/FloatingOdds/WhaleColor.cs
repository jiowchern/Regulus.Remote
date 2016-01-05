using System.Collections.Generic;
using System.Linq;

using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds
{
	/// <summary>
	///     七彩鲸 的倍数
	/// </summary>
	public class WhaleColor : IFloatingCalculator
	{
		private readonly List<OddsData> _OddsDatas;

		public WhaleColor()
		{
			_OddsDatas = new List<OddsData>
			{
				new OddsData
				{
					Odds = 300, 
					Rate = 0.1f
				}, 
				new OddsData
				{
					Odds = 350, 
					Rate = 0.2f
				}, 
				new OddsData
				{
					Odds = 400, 
					Rate = 0.3f
				},
				new OddsData
				{
					Odds = 400,
					Rate = 0.4f
				},
				new OddsData
				{
					Odds = 500, 
					Rate = 0.5f
				}, 
				new OddsData
				{
					Odds = 550, 
					Rate = 0.6f
				}, 
				new OddsData
				{
					Odds = 600, 
					Rate = 0.7f
				}
			};
		}

		void IFloatingCalculator.Calculate(RequsetFishData[] fish_data)
		{
			var whales = fish_data.Where(x => x.FishType == FISH_TYPE.WHALE_COLOR);

			foreach(var whale in whales)
			{
				var random = Regulus.Utility.Random.Instance.NextFloat(0.1f, 0.7f);
				whale.FishOdds = _OddsDatas.Find(x => x.Rate >= random).Odds;
			}
		}
	}
}