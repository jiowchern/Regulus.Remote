using System.Collections.Generic;
using System.Linq;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule;

namespace VGame.Project.FishHunter.Formula.ZsFormula
{
	public class ZsHitChecker : HitBase
	{
		private readonly DataVisitor _DataVisitor;

		public ZsHitChecker(FishFarmData fish_farm_data, FormulaPlayerRecord formula_player_record, List<RandomData> random)
		{
			var record = formula_player_record.FindFarmRecord(fish_farm_data.FarmId);

            if(record == null)
            {
				var tmp = formula_player_record.FarmRecords.ToList();
				tmp.Add(new FarmRecord(fish_farm_data.FarmId));
				formula_player_record.FarmRecords = tmp.ToArray();
			}

			_DataVisitor = new DataVisitor(fish_farm_data, formula_player_record, random);
		}

		public override HitResponse[] TotalRequest(HitRequest request)
		{
			var block = CalculationBufferBlock.GetBlock(request, _DataVisitor.Farm.MaxBet);

			_DataVisitor.FocusBlockName = block;

			if(request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL)
			{
				new AccumulationBufferRule(_DataVisitor, request).Run();

				new ApproachBaseOddsRule(_DataVisitor).Run();

				new AdjustmentAverageRule(_DataVisitor, request).Run();
			}

			new AdjustmentGameLevelRule(_DataVisitor).Run();

			new AdjustmentPlayerPhaseRule(_DataVisitor).Run();

			return new DeathRule(_DataVisitor, request).Run();
		}
	}
}
