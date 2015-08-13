using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula
{
	public struct ExpansionFeature
	{
		public IFormulaStageDataRecorder FormulaStageDataRecorder { get; private set; }

		public IAccountFinder AccountFinder { get; private set; }

		public IFormulaPlayerRecorder FormulaPlayerRecorder { get; private set; }

		public ExpansionFeature(
			IAccountFinder account_finder, 
			IFormulaStageDataRecorder formula_stage_data_recorder, 
			IFormulaPlayerRecorder formula_player_recorder) : this()
		{
			AccountFinder = account_finder;
			FormulaStageDataRecorder = formula_stage_data_recorder;
			FormulaPlayerRecorder = formula_player_recorder;
		}
	}
}
