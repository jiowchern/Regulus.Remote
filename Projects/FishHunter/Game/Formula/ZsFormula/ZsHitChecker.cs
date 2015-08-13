using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule;

namespace VGame.Project.FishHunter.Formula.ZsFormula
{
	public class ZsHitChecker : HitBase
	{
		private readonly StageDataVisitor _StageVisitor;

		public ZsHitChecker(StageData stage_data, IFormulaStageDataRecorder formula_stage_data_recorder, IFormulaPlayerRecorder formula_player_recorder)
		{
			_StageVisitor = new StageDataVisitor(stage_data, formula_player_recorder, formula_stage_data_recorder);
		}

		
		public override HitResponse Request(HitRequest request)
		{
//			var block = CalculationBufferBlock.GetBlock(request.WeaponData.WepBet, _StageVisitor.FocusStageData.MaxBet);
//
//			_StageVisitor.FocusBufferBlock = (StageBuffer.BUFFER_BLOCK)block;
//
//			if(request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL
//			   || request.WeaponData.WeaponType == WEAPON_TYPE.FREE_POWER)
//			{
//				// 会拿到所有的道具(武器)
//				return _CheckIsFirstFire(request);
//			}
//
//			// 特武打死的鱼不会拿到2、3、4道具
//			return new SpecialWeaponRule(_StageVisitor, request).Run();
			return new HitResponse();
		}

		public override HitResponse[] TotalRequest(HitRequest request)
		{
			var block = CalculationBufferBlock.GetBlock(request, _StageVisitor.FocusStageData.MaxBet);

			_StageVisitor.FocusBufferBlock = (StageBuffer.BUFFER_BLOCK)block;


			/// 只有第一發才能累積buffer
			if (request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL && request.WeaponData.TotalHits == 1)
			{
				new AccumulationBufferRule(_StageVisitor, request).Run();

				new ApproachBaseOddsRule(_StageVisitor).Run();

				new AdjustmentAverageRule(_StageVisitor, request).Run();
			}

			new AdjustmentGameLevelRule(_StageVisitor).Run();

			new AdjustmentPlayerPhaseRule(_StageVisitor).Run();


			return new DeathRule(_StageVisitor, request).Run();

			// 特武打死的鱼不会拿到2、3、4道具
			//return new SpecialWeaponRule(_StageVisitor, request).Run();
		}

	}
}
