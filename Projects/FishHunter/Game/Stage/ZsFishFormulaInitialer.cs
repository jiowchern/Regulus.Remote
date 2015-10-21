using System;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula.ZsFormula;

namespace VGame.Project.FishHunter.Stage
{
	internal class ZsFishFormulaInitialer
	{
		private readonly IFormulaFarmRecorder _FormulaFarmRecorder;

		private readonly IFormulaPlayerRecorder _FormulaPlayerRecorder;

		public ZsFishFormulaInitialer(
			IFormulaPlayerRecorder formula_player_recorder, 
			IFormulaFarmRecorder formula_farm_recorder)
		{
			_FormulaPlayerRecorder = formula_player_recorder;
			_FormulaFarmRecorder = formula_farm_recorder;
		}

		public Value<IFishStage> Query(Guid player_id, FishFarmData data)
		{
			var val = new Value<IFishStage>();

			_FormulaPlayerRecorder.Query(player_id).OnValue += record =>
			{
				val.SetValue(
					new ZsFishStage(
						player_id, 
						data, 
						record, 
						_FormulaPlayerRecorder, 
						_FormulaFarmRecorder));
			};

			return val;
		}
	}
}
