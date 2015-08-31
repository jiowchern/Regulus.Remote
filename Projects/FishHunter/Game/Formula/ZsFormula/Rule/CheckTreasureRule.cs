using System.Collections.Generic;
using System.Linq;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class CheckTreasureRule
	{
		private readonly DataVisitor _DataVisitor;

		private readonly RequsetFishData _FishData;

		private List<WEAPON_TYPE> _GotTreasures;

		public CheckTreasureRule(DataVisitor data_visitor, RequsetFishData fish_data)
		{
			_DataVisitor = data_visitor;
			_FishData = fish_data;
			_GotTreasures = new List<WEAPON_TYPE>();
        }

		/// <summary>
		///     是否取得特殊道具（特殊武器
		/// </summary>
		public void Run()
		{
			_CheckCertain();

			_CheckRandom();

			_SavePlayerHit();

			_DataVisitor.GotTreasures = _GotTreasures;
		}

		/// <summary>
		///     檢查必掉道具
		/// </summary>
		private void _CheckCertain()
		{
			if(_FishData.FishType >= FISH_TYPE.TROPICAL_FISH && _FishData.FishType <= FISH_TYPE.WHALE_COLOR)
			{
				_GotTreasures.Add(
					_FishData.FishStatus == FISH_STATUS.KING
						? WEAPON_TYPE.KING
						: WEAPON_TYPE.INVALID);
			}
			else
			{
				var certainWeapons = FishTreasure.Get().Find(x => x.FishType == _FishData.FishType).CertainWeapons;

				foreach(var weapon in certainWeapons)
				{
					_GotTreasures.Add(weapon);
				}
			}
		}

		/// <summary>
		///     檢查隨機道具
		/// </summary>
		private void _CheckRandom()
		{
			// 拿到魚的掉落物品清單
			var randomWeapons = FishTreasure.Get().Find(x => x.FishType == _FishData.FishType).RandomWeapons;

			var bufferData = _DataVisitor.Farm.FindBuffer(_DataVisitor.FocusBufferBlock, FarmBuffer.BUFFER_TYPE.SPEC);

			var list = new List<WEAPON_TYPE>();

			// 計算魚掉那個寶
			foreach(var t in randomWeapons)
			{
				long gate = bufferData.Rate / randomWeapons.Length;

				var dd = new SpecialWeaponPowerTable().WeaponPowers.FirstOrDefault(x => x.WeaponType == t);

				var power = new SpecialWeaponPowerTable().WeaponPowers.Find(x => x.WeaponType == t).Power;

				gate = (0x0FFFFFFF / power) * gate;

				gate = gate / 1000;

				if(bufferData.BufferTempValue.HiLoRate >= 0)
				{
					gate *= 2;
				}

				if(bufferData.BufferTempValue.HiLoRate < -200)
				{
					gate /= 2;
				}

				var randomValue = _DataVisitor.FindIRandom(RandomData.RULE.CHECK_TREASURE, 0).NextInt(0, 0x10000000);
				if(randomValue >= gate)
				{
					continue;
				}

				// 在這隻魚身上得到的道具
				list.Add(t);
			}

			OrderByWeapon(list);
		}

		private void OrderByWeapon(IEnumerable<WEAPON_TYPE> list)
		{
			var weaponrandomValue = _DataVisitor.FindIRandom(RandomData.RULE.CHECK_TREASURE, 1).NextFloat(0, 1);

			var randomWeapon = list.OrderBy(x => weaponrandomValue).FirstOrDefault();

			if(randomWeapon != WEAPON_TYPE.INVALID)
			{
				_GotTreasures.Add(randomWeapon);
			}
		}

		private void _SavePlayerHit()
		{
			var fishHitRecord =
				_DataVisitor.PlayerRecord.FindFarmRecord(_DataVisitor.Farm.FarmId)
							.FishHits.First(x => x.FishType == _FishData.FishType);

			fishHitRecord.Datas = _GotTreasures.Select(
				treasure => new FishHitRecord.TreasureData
				{
					WeaponType = treasure, 
					Count = 1
				}).ToArray();
		}
	}
}
