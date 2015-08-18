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
		private readonly StageDataVisitor _StageVisitor;

		public ApproachBaseOddsRule(StageDataVisitor stage_visitor)
		{
			_StageVisitor = stage_visitor;
		}

		/// <summary>
		///     NowBaseOdds 趨近 SetBaseOdds
		/// </summary>
		public void Run()
		{
			if(_StageVisitor.FocusFishFarmData.BaseOddsCount != 0)
			{
				_StageVisitor.FocusFishFarmData.BaseOddsCount--;
			}
			else
			{
				_StageVisitor.FocusFishFarmData.BaseOddsCount = 3000;
				if(_StageVisitor.FocusFishFarmData.NowBaseOdds > _StageVisitor.FocusFishFarmData.BaseOdds)
				{
					_StageVisitor.FocusFishFarmData.NowBaseOdds--;
				}
				else if(_StageVisitor.FocusFishFarmData.NowBaseOdds < _StageVisitor.FocusFishFarmData.BaseOdds)
				{
					_StageVisitor.FocusFishFarmData.NowBaseOdds++;
				}
			}
		}
	}
}
