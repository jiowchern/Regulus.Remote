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

		public CheckTreasureRule(DataVisitor data_visitor, RequsetFishData fish_data)
		{
			_DataVisitor = data_visitor;
			_FishData = fish_data;
		}

		/// <summary>
		///     是否取得特殊道具（特殊武器
		/// </summary>
		public void Run()
		{
			_CheckCertain();

			_CheckRandom();

			_CheckCount();

			_SavePlayerHit();
		}

		private void _SavePlayerHit()
		{
			var fishHitRecord =
				_DataVisitor.PlayerRecord.FindFarmRecord(_DataVisitor.Farm.FarmId)
							.FishHits.First(x => x.FishType == _FishData.FishType);

			fishHitRecord.Records = _DataVisitor.GotTreasures.Select(
				treasure => new FishHitRecord.TreasureRecord
				{
					WeaponType = treasure, 
					Count = 1
				}).ToArray();
		}

		private void _CheckCount()
		{
			if(_DataVisitor.GotTreasures.Count == 0)
			{
				_DataVisitor.GotTreasures.Add(WEAPON_TYPE.INVALID);
			}
		}

		/// <summary>
		///     檢查必掉道具
		/// </summary>
		private void _CheckCertain()
		{
			var certainWeapons = new FishTreasure().Treasures.Find(x => x.FishType == _FishData.FishType).CertainWeapons;

			if(certainWeapons == WEAPON_TYPE.INVALID)
			{
				return;
			}

			_DataVisitor.GotTreasures.Add(certainWeapons);
		}

		/// <summary>
		///     檢查隨機道具
		/// </summary>
		private void _CheckRandom()
		{
			// 拿到魚的掉落物品清單
			var randomWeapons = new FishTreasure().Treasures.Find(x => x.FishType == _FishData.FishType).RandomWeapons;

			var bufferData = _DataVisitor.Farm.FindBuffer(_DataVisitor.FocusBufferBlock, FarmBuffer.BUFFER_TYPE.SPEC);

			var list = new List<WEAPON_TYPE>();

			// 計算魚掉那個寶
			foreach(var t in randomWeapons)
			{
				long gate = bufferData.Rate / randomWeapons.Length;

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

				if(_DataVisitor.Random.NextInt(0, 0x10000000) >= gate)
				{
					continue;
				}

				// 在這隻魚身上得到的道具
				list.Add(t);
			}

			var randomWeapon = list.OrderBy(x => _DataVisitor.Random.NextFloat(0, 1)).FirstOrDefault();

			if(randomWeapon != WEAPON_TYPE.INVALID)
			{
				_DataVisitor.GotTreasures.Add(randomWeapon);
			}
		}
	}
}
