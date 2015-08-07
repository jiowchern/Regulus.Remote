
using System;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula;
using VGame.Project.FishHunter.ZsFormula.Data;
using VGame.Project.FishHunter.ZsFormula.Rule;

namespace VGame.Project.FishHunter.ZsFormula
{
	public class ZsHitChecker : HitBase
	{
		private readonly PlayerVisitor _PlayerVisitor;

		private readonly FishStageVisitor _StageVisitor;

		public ZsHitChecker(StageData stage_data)
		{
			_PlayerVisitor = new PlayerVisitor(new Guid(), stage_data.StageId);

			_StageVisitor = new FishStageVisitor(stage_data);
		}

		/// <summary>
		/// // 只有第一發才能累積buffer
		/// </summary>
		/// <param name="request">
		/// </param>
		/// <returns>
		/// The <see cref="HitResponse"/>.
		/// </returns>
		private HitResponse _CheckIsFirstFire(HitRequest request)
		{
			if (request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL && request.WeaponData.TotalHits == 1)
			{
				new AccumulationBufferRule(_StageVisitor, request, _PlayerVisitor.FocusPlayer).Run();

				new ApproachBaseOddsRule(_StageVisitor).Run();

				new AdjustmentAverageRule(_StageVisitor, request).Run();
			}

			new AdjustmentGameLevelRule(_StageVisitor).Run();

			new AdjustmentPlayerPhaseRule(_StageVisitor, _PlayerVisitor.FocusPlayer).Run();

			new AcquisitionSpecialWeaponRule(_StageVisitor, _PlayerVisitor.FocusPlayer).Run();

			return new DeathRule(_StageVisitor, request, _PlayerVisitor.FocusPlayer).Run();
		}

		public override HitResponse Request(HitRequest request)
		{
			var block = CalculationBufferBlock.GetBlock(request.WeaponData.WepBet, _StageVisitor.NowData.MaxBet);
			
			_StageVisitor.NowBlock = (StageBuffer.BUFFER_BLOCK)block;

			if (request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL
			    || request.WeaponData.WeaponType == WEAPON_TYPE.FREE_POWER)
			{
				return _CheckIsFirstFire(request);
			}

			return new SpecialWeaponRule(_StageVisitor, request, _PlayerVisitor.FocusPlayer).Run();
		}
	}
}