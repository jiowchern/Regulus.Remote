using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation;

namespace VGame.Project.FishHunter.Formula.ZsFormula
{
	public class ZsHitChecker : HitBase
	{
		private readonly DataVisitor _DataVisitor;

		public ZsHitChecker(FishFarmData fish_farm_data, FormulaPlayerRecord formula_player_record)
		{
			var record = formula_player_record.FindFarmRecord(fish_farm_data.FarmId);

			if(record == null)
			{
				var tmp = formula_player_record.FarmRecords.ToList();
				tmp.Add(new FarmRecord(fish_farm_data.FarmId));
				formula_player_record.FarmRecords = tmp.ToArray();
			}

			_DataVisitor = new DataVisitor(fish_farm_data, formula_player_record);
		}

		public override HitResponse[] TotalRequest(HitRequest request)
		{
			var handler = new HitPipelineHandler(_DataVisitor, request);

			return handler.SetFocusBlock()
						.SetFishDieRateFromHitOrder()
						.OddsCalculate()
						.FloatingOddsActuate()
						.SpecialWeaponSelect()
						.LotteryTreasure()
						.AccumulationBuffer()
						.ApproachBaseOdds()
						.AdjustmentAverage()
						.AdjustmentGameLevel()
						.AdjustmentPlayerPhase()
						.DieRateCalculate()
						.Record()
						.Log()
						.Process()
						.ToArray();
		}
	}
}
