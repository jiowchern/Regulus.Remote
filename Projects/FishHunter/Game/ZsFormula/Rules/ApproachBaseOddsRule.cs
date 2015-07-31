// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApproachBaseOddsRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the ApproachBaseOddsRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	public class ApproachBaseOddsRule
	{
		private readonly StageDataVisit _StageDataVisit;

		public ApproachBaseOddsRule(StageDataVisit stage_data_visit)
		{
			_StageDataVisit = stage_data_visit;
		}

		/// <summary>
		/// 让 NowBaseOdds 趋近 SetBaseOdds
		/// </summary>
		public void Run()
		{
			if (_StageDataVisit.NowUseData.BaseOddsCount != 0)
			{
				_StageDataVisit.NowUseData.BaseOddsCount--;
			}
			else
			{
				_StageDataVisit.NowUseData.BaseOddsCount = 3000;
				if (_StageDataVisit.NowUseData.NowBaseOdds > _StageDataVisit.NowUseData.BaseOdds)
				{
					_StageDataVisit.NowUseData.NowBaseOdds--;
				}
				else if (_StageDataVisit.NowUseData.NowBaseOdds < _StageDataVisit.NowUseData.BaseOdds)
				{
					_StageDataVisit.NowUseData.NowBaseOdds++;
				}
			}
		}
	}
}