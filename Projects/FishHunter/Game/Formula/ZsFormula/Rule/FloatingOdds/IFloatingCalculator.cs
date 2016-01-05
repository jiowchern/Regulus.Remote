using System;

using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds
{
	public interface IFloatingCalculator
	{
		void Calculate(RequsetFishData[] fish_data);
	}

}