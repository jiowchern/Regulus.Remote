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
		private readonly DataVisitor _Visitor;

		public ApproachBaseOddsRule(DataVisitor visitor)
		{
			_Visitor = visitor;
		}

		/// <summary>
		///     NowBaseOdds 趨近 SetBaseOdds
		/// </summary>
		public void Run()
		{
			if(_Visitor.Farm.BaseOddsCount != 0)
			{
				_Visitor.Farm.BaseOddsCount--;
			}
			else
			{
				_Visitor.Farm.BaseOddsCount = 3000;
				if(_Visitor.Farm.NowBaseOdds > _Visitor.Farm.BaseOdds)
				{
					_Visitor.Farm.NowBaseOdds--;
				}
				else if(_Visitor.Farm.NowBaseOdds < _Visitor.Farm.BaseOdds)
				{
					_Visitor.Farm.NowBaseOdds++;
				}
			}
		}
	}
}
