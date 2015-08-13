using System.Collections.Generic;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
	public class StageDataVisitor
	{
		public IFormulaStageDataRecorder FormulaStageDataRecorder { get; private set; }

		public IFormulaPlayerRecorder FormulaPlayerRecorder { get; private set; }

		public FormulaPlayerRecord PlayerRecord { get; set; }

		public StageData FocusStageData { get; private set; }

		public StageBuffer.BUFFER_BLOCK FocusBufferBlock { get; set; }

		public List<WEAPON_TYPE> GetItems;

		public StageDataVisitor(StageData stage_data, IFormulaPlayerRecorder formula_player_recorder, IFormulaStageDataRecorder formula_stage_data_recorder)
		{
			FocusStageData = stage_data;
			FormulaStageDataRecorder = formula_stage_data_recorder;
			FormulaPlayerRecorder = formula_player_recorder;
		}
	}
}
