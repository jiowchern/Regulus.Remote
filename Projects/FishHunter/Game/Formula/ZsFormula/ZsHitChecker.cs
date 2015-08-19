
using System.Collections.Generic;


using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule;

namespace VGame.Project.FishHunter.Formula.ZsFormula
{
	public class ZsHitChecker : HitBase
	{
		private readonly FormulaPlayerRecord _FormulaPlayerRecord;

		private readonly FarmDataVisitor _FarmVisitor;

		public ZsHitChecker(FishFarmData fish_farm_data, FormulaPlayerRecord formula_player_record, IRandom random)
		{
			_FormulaPlayerRecord = formula_player_record;

		    var data = new List<FarmRecord>
		    {
		        fish_farm_data.RecordData
		    };

		    _FormulaPlayerRecord.StageRecords = data.ToArray();

			_FarmVisitor = new FarmDataVisitor(fish_farm_data, _FormulaPlayerRecord, random);
		}

		public override HitResponse[] TotalRequest(HitRequest request)
		{
			var block = CalculationBufferBlock.GetBlock(request, _FarmVisitor.FocusFishFarmData.MaxBet);

			_FarmVisitor.FocusBufferBlock = block;

			// 只有第一發才能累積buffer
			if(request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL && request.WeaponData.TotalHits == 1)
			{
				new AccumulationBufferRule(_FarmVisitor, request).Run();

				new ApproachBaseOddsRule(_FarmVisitor).Run();

				new AdjustmentAverageRule(_FarmVisitor, request).Run();
			}

			new AdjustmentGameLevelRule(_FarmVisitor).Run();

			new AdjustmentPlayerPhaseRule(_FarmVisitor).Run();

			return new DeathRule(_FarmVisitor, request).Run();
		}
	}
}
