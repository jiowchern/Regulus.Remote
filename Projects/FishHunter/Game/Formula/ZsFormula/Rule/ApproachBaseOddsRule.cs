using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class ApproachBaseOddsRule
	{
		private const int _CheckNumbers = 10000;

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
			if(_Visitor.Farm.NowBaseOdds >= _Visitor.Farm.BaseOdds)
			{
				return;
			}

			if(_Visitor.Farm.Record.FireCount % ApproachBaseOddsRule._CheckNumbers == 0)
			{
				_Visitor.Farm.NowBaseOdds += 1;
			}
		}
	}
}
