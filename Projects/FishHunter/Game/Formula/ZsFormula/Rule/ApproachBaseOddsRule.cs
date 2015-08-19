// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApproachBaseOddsRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the ApproachBaseOddsRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class ApproachBaseOddsRule
	{
		private readonly FarmDataVisitor _FarmVisitor;

		public ApproachBaseOddsRule(FarmDataVisitor farm_visitor)
		{
			_FarmVisitor = farm_visitor;
		}

		/// <summary>
		///     NowBaseOdds 趨近 SetBaseOdds
		/// </summary>
		public void Run()
		{
			if(_FarmVisitor.FocusFishFarmData.BaseOddsCount != 0)
			{
				_FarmVisitor.FocusFishFarmData.BaseOddsCount--;
			}
			else
			{
				_FarmVisitor.FocusFishFarmData.BaseOddsCount = 3000;
				if(_FarmVisitor.FocusFishFarmData.NowBaseOdds > _FarmVisitor.FocusFishFarmData.BaseOdds)
				{
					_FarmVisitor.FocusFishFarmData.NowBaseOdds--;
				}
				else if(_FarmVisitor.FocusFishFarmData.NowBaseOdds < _FarmVisitor.FocusFishFarmData.BaseOdds)
				{
					_FarmVisitor.FocusFishFarmData.NowBaseOdds++;
				}
			}
		}
	}
}
