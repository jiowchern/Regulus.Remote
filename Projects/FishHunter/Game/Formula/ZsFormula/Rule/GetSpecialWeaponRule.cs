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
		private readonly FarmDataVisitor _FarmVisitor;

		private readonly RequsetFishData _FishData;

		public GetSpecialWeaponRule(FarmDataVisitor farm_visitor, RequsetFishData fish_data)
		{
			_FarmVisitor = farm_visitor;
			_FishData = fish_data;
		}

		/// <summary>
		///     是否取得特殊道具（特殊武器
		/// </summary>
		public void Run()
		{
			// todo 如道具可以累積獲得，這個判斷就有問題，原版公式的意思就是只能一次一個
//			if (_FarmVisitor.PlayerRecord.NowWeaponPower.HaveWeapon)
//			{
//				return;
//			}

			_CheckCertain();

			_CheckRandom();

		    _CheckCount();
		}

	    private void _CheckCount()
	    {
	        if(_FarmVisitor.GetItems.Count == 0)
	        {
                _FarmVisitor.GetItems.Add(WEAPON_TYPE.INVALID);
            }
        }

	    /// <summary>
        /// 檢查必掉道具
        /// </summary>
		private void _CheckCertain()
		{
			var certainWeapons =
				_FarmVisitor.PlayerRecord.FindFarmRecord(_FarmVisitor.FocusFishFarmData.FarmId)
				            .FishHitReuslt.Items.First(x => x.FishType == _FishData.FishType).CertainWeapons;

			if (certainWeapons != WEAPON_TYPE.INVALID)
			{
				_FarmVisitor.GetItems.Add(certainWeapons);
			}
		}

        /// <summary>
        /// 檢查隨機道具
        /// </summary>
		private void _CheckRandom()
		{
			// 拿到魚的掉落物品清單
			var randomWeapons =
				_FarmVisitor.PlayerRecord.FindFarmRecord(_FarmVisitor.FocusFishFarmData.FarmId)
				            .FishHitReuslt.Items.First(x => x.FishType == _FishData.FishType).RandomWeapons;

			var bufferData = _FarmVisitor.FocusFishFarmData.FindBuffer(_FarmVisitor.FocusBufferBlock, FarmBuffer.BUFFER_TYPE.SPEC);

			System.Collections.Generic.List<WEAPON_TYPE> list = new List<WEAPON_TYPE>();
			// 計算魚掉那個寶
			foreach (var t in randomWeapons)
			{
				long gate = bufferData.Rate / randomWeapons.Length;

				var power = new SpecialWeaponPowerTable().WeaponPowers.Find(x => x.WeaponType == t).Power;

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

				if (_FarmVisitor.Random.NextInt(0, 0x10000000) >= gate)
				{
					continue;
				}

				// 在這隻魚身上得到的道具
				list.Add(t);
			}

			var randomWeapon = list.OrderBy(x => _FarmVisitor.Random.NextFloat(0, 1)).FirstOrDefault();

		    if(randomWeapon != WEAPON_TYPE.INVALID)
		    {
		        _FarmVisitor.GetItems.Add(randomWeapon);
		    }
		}
	}
}
