
using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	public class AcquisitionSpecialWeaponRule
	{
		private readonly PlayerRecord _PlayerRecord;

		private readonly FishStageVisitor _StageVisitor;

		public AcquisitionSpecialWeaponRule(FishStageVisitor stage_visitor, PlayerRecord player_record)
		{
			_StageVisitor = stage_visitor;
			_PlayerRecord = player_record;
		}

		/// <summary>
		///     是否取得特殊道具（特殊武器）
		/// </summary>
		public void Run()
		{
			if (_PlayerRecord.NowSpecialWeaponData.HaveWeapon)
			{
				return;
			}

			var specialWeaponDatas = _PlayerRecord.FindStageRecord(_StageVisitor.NowData.StageId).SpecialWeaponDatas;
			foreach (var specialWeaponData in specialWeaponDatas)
			{
				var bufferData = _StageVisitor.NowData.FindBuffer(_StageVisitor.NowBlock, StageBuffer.BUFFER_TYPE.SPEC);

				var gate = bufferData.Rate / specialWeaponDatas.Count;
				gate = (0x0FFFFFFF / (int)specialWeaponData.Power) * gate;
				gate = gate / 1000;
				if (bufferData.BufferTempValue.HiLoRate >= 0)
				{
					gate *= 2;
				}

				if (bufferData.BufferTempValue.HiLoRate < -200)
				{
					gate /= 2;
				}

				var rand = Random.Instance.NextInt(0, 0x10000000);
				if (rand >= gate)
				{
					continue;
				}

				specialWeaponData.HaveWeapon = true;
				_PlayerRecord.NowSpecialWeaponData = specialWeaponData;

				break;
			}
		}
	}
}