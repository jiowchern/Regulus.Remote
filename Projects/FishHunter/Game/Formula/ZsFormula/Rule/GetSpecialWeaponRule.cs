// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSpecialWeaponRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the GetSpecialWeaponRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class GetSpecialWeaponRule
	{
		private readonly StageDataVisitor _StageVisitor;

		private readonly RequsetFishData _FishData;

		public GetSpecialWeaponRule(StageDataVisitor stage_visitor, RequsetFishData fish_data)
		{
			_StageVisitor = stage_visitor;
			_FishData = fish_data;
		}

		/// <summary>
		///     是否取得特殊道具（特殊武器
		/// </summary>
		public void Run()
		{
			// todo 如道具可以累積獲得，這個判斷就有問題，原版公式的意思就是只能一次一個
//			if (_StageVisitor.PlayerRecord.NowWeaponPower.HaveWeapon)
//			{
//				return;
//			}

			//拿到魚的掉落物品清單
			var fishOwnItems =
				_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusStageData.StageId)
				             .FishHitReuslt.FishToItems.Find(x => x.FishType == _FishData.FishType)
				             .OwnWeapons;

			var bufferData = _StageVisitor.FocusStageData.FindBuffer(_StageVisitor.FocusBufferBlock, StageBuffer.BUFFER_TYPE.SPEC);


			// 計算魚掉那個寶
			for(int i = 0; i < fishOwnItems.Length; ++i)
			{
				var gate = bufferData.Rate / fishOwnItems.Length;

				var power = new WeaponPowerTable().WeaponPowers.Find(x => x.WeaponType == fishOwnItems[i]).Power;
				
				gate = (0x0FFFFFFF / power) * gate;
				
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
				
				// 在這隻魚身上得到的道具
				_StageVisitor.GetItems.Add(fishOwnItems[i]);
			}

//			foreach(var specialWeaponData in specialWeaponDatas)
//			{
//				var bufferData = _StageVisitor.FocusStageData.FindBuffer(_StageVisitor.FocusBufferBlock, StageBuffer.BUFFER_TYPE.SPEC);
//
//				var gate = bufferData.Rate / specialWeaponDatas.Count;
//
//				gate = (0x0FFFFFFF / (int)specialWeaponData.Power) * gate;
//				
//				gate = gate / 1000;
//				
//				if(bufferData.BufferTempValue.HiLoRate >= 0)
//				{
//					gate *= 2;
//				}
//
//				if(bufferData.BufferTempValue.HiLoRate < -200)
//				{
//					gate /= 2;
//				}
//
//				var rand = Random.Instance.NextInt(0, 0x10000000);
//				if(rand >= gate)
//				{
//					continue;
//				}
//
//				specialWeaponData.HaveWeapon = true;
//				_StageVisitor.PlayerRecord.NowWeaponPower = specialWeaponData;
//
//				break;
//			}
		}
	}
}
