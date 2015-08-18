// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSpecialWeaponRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the GetSpecialWeaponRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;


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

			// 檢查一定得到的物品
			_CheckCertain();

			_CheckRandom();
		}

		private void _CheckCertain()
		{
			var CertainWeapons =
				_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusFishFarmData.FarmId)
							 .FishHitReuslt.Items.Find(x => x.FishType == _FishData.FishType).CertainWeapons;
			if (CertainWeapons != WEAPON_TYPE.INVALID)
			{
				_StageVisitor.GetItems.Add(CertainWeapons);
			}
		}

		private void _CheckRandom()
		{
			// 拿到魚的掉落物品清單
			var randomWeapons =
				_StageVisitor.PlayerRecord.FindStageRecord(_StageVisitor.FocusFishFarmData.FarmId)
							 .FishHitReuslt.Items.Find(x => x.FishType == _FishData.FishType).RandomWeapons;

			var bufferData = _StageVisitor.FocusFishFarmData.FindBuffer(_StageVisitor.FocusBufferBlock, FarmBuffer.BUFFER_TYPE.SPEC);

			System.Collections.Generic.List<WEAPON_TYPE> list = new List<WEAPON_TYPE>();
			// 計算魚掉那個寶
			foreach (var t in randomWeapons)
			{
				long gate = bufferData.Rate / randomWeapons.Length;

				var power = new WeaponPowerTable().WeaponPowers.Find(x => x.WeaponType == t).Power;

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

				if (_StageVisitor.Random.NextInt(0, 0x10000000) >= gate)
				{
					continue;
				}

				// 在這隻魚身上得到的道具
				list.Add(t);
			}

			var randomWeapon = list.OrderBy(x => _StageVisitor.Random.NextFloat(0, 1)).FirstOrDefault();
			if(randomWeapon != WEAPON_TYPE.INVALID)
				_StageVisitor.GetItems.Add(randomWeapon);
		}
	}
}
