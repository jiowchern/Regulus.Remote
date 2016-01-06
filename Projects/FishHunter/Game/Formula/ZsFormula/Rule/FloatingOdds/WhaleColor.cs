﻿using System.Collections.Generic;
using System.Linq;

using Regulus.Utility;

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
					Rate = 0
				}, 
				new OddsData
				{
					Odds = 350, 
					Rate = 1
				}, 
				new OddsData
				{
					Odds = 400, 
					Rate = 2
				}, 
				new OddsData
				{
					Odds = 400, 
					Rate = 3
				}, 
				new OddsData
				{
					Odds = 500, 
					Rate = 4
				}, 
				new OddsData
				{
					Odds = 550, 
					Rate = 5
				}, 
				new OddsData
				{
					Odds = 600, 
					Rate = 6
				}
			};
		}

		void IFloatingCalculator.Calculate(RequsetFishData[] fish_data)
		{
			var whales = fish_data.Where(x => x.FishType == FISH_TYPE.WHALE_COLOR);

			foreach(var whale in whales)
			{
				var random = Random.Instance.NextInt(0, 7);
				whale.FishOdds = _OddsDatas.Find(x => x.Rate == random)
											.Odds;
			}
		}
	}
}
