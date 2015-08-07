
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	public class ApproachBaseOddsRule
	{
		private readonly FishStageVisitor _StageVisitor;

		public ApproachBaseOddsRule(FishStageVisitor stage_visitor)
		{
			_StageVisitor = stage_visitor;
		}

		/// <summary>
		///     让 NowBaseOdds 趋近 SetBaseOdds
		/// </summary>
		public void Run()
		{
			if (_StageVisitor.NowData.BaseOddsCount != 0)
			{
				_StageVisitor.NowData.BaseOddsCount--;
			}
			else
			{
				_StageVisitor.NowData.BaseOddsCount = 3000;
				if (_StageVisitor.NowData.NowBaseOdds > _StageVisitor.NowData.BaseOdds)
				{
					_StageVisitor.NowData.NowBaseOdds--;
				}
				else if (_StageVisitor.NowData.NowBaseOdds < _StageVisitor.NowData.BaseOdds)
				{
					_StageVisitor.NowData.NowBaseOdds++;
				}
			}
		}
	}
}