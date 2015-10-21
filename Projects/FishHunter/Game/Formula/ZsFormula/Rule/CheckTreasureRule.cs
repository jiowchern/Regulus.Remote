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

		private readonly List<WEAPON_TYPE> _GotTreasures;

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
			if(_FishData.FishType >= FISH_TYPE.TROPICAL_FISH && _FishData.FishType <= FISH_TYPE.SPECIAL_EAT_FISH_CRAZY)
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

			var farmDataRoot = _DataVisitor.Farm.FindDataRoot(
				_DataVisitor.FocusBlockName, 
				FarmDataRoot.BufferNode.BUFFER_NAME.SPEC);

			var list = new List<WEAPON_TYPE>();

			// 計算魚掉那個寶
			foreach(var t in randomWeapons)
			{
				long gate = farmDataRoot.Buffer.Rate / randomWeapons.Length;

				var rate = new SpecialWeaponRateTable().WeaponRates.Find(x => x.WeaponType == t).Rate;

				gate = (0x0FFFFFFF / rate) * gate;

				gate = gate / 1000;

				if(farmDataRoot.TempValueNode.HiLoRate >= 0)
				{
					gate *= 2;
				}

				if(farmDataRoot.TempValueNode.HiLoRate < -200)
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
			var fishHitRecords = _DataVisitor.PlayerRecord.FindFarmRecord(_DataVisitor.Farm.FarmId).FishHits;

			var record = fishHitRecords.FirstOrDefault(x => x.FishType == _FishData.FishType);

			if(record == null)
			{
				return;
			}

			var list = record.TreasureDatas.ToList();

			foreach(var t in _GotTreasures.Select(
				treasure => list.FirstOrDefault(x => x.WeaponType == treasure)
							?? new FishHitRecord.TreasureData(treasure)))
			{
				list.Add(t);
				t.Count++;
			}

			record.TreasureDatas = list.ToArray();
		}
	}
}
