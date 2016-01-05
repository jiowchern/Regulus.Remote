using System;
using System.Linq;

using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds
{
	public class BigOctopus : IFloatingCalculator
	{
		void IFloatingCalculator.Calculate(RequsetFishData[] fish_data)
		{
			var octs = fish_data.Where(x => x.FishType == FISH_TYPE.SPECIAL_BIG_OCTOPUS_BOMB);

			foreach(var oct in octs)
			{
				oct.FishOdds += oct.GraveGoods.Sum(x => x.FishOdds);
			}
		}
	}
}
