
using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule;

namespace VGame.Project.FishHunter.Formula.ZsFormula
{
	public class ZsHitChecker : HitBase
	{
		private readonly FormulaPlayerRecord _FormulaPlayerRecord;

		private readonly StageDataVisitor _StageVisitor;

		public ZsHitChecker(FishFarmData fish_farm_data, FormulaPlayerRecord formula_player_record, IRandom random)
		{
			_FormulaPlayerRecord = formula_player_record;

			_FormulaPlayerRecord.StageRecords.Add(fish_farm_data.RecordData);
			_StageVisitor = new StageDataVisitor(fish_farm_data, _FormulaPlayerRecord, random);
		}

		public override HitResponse Request(HitRequest request)
		{
			return new HitResponse();
		}

		public override HitResponse[] TotalRequest(HitRequest request)
		{
			var block = CalculationBufferBlock.GetBlock(request, _StageVisitor.FocusFishFarmData.MaxBet);

			_StageVisitor.FocusBufferBlock = block;

			// 只有第一發才能累積buffer
			if(request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL && request.WeaponData.TotalHits == 1)
			{
				new AccumulationBufferRule(_StageVisitor, request).Run();

				new ApproachBaseOddsRule(_StageVisitor).Run();

				new AdjustmentAverageRule(_StageVisitor, request).Run();
			}

			new AdjustmentGameLevelRule(_StageVisitor).Run();

			new AdjustmentPlayerPhaseRule(_StageVisitor).Run();

			return new DeathRule(_StageVisitor, request).Run();
		}
	}
}
