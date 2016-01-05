using System;
using System.Collections.Generic;
using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	/// <summary>
	///     檢查隨機道具，一發子彈檢查一次
	/// </summary>
	public class LotteryTreasureTrigger : IPipelineElement
	{
		private readonly DataVisitor _DataVisitor;

		private readonly List<WEAPON_TYPE> _GotTreasures;

		public LotteryTreasureTrigger(DataVisitor data_visitor)
		{
			_DataVisitor = data_visitor;
			_GotTreasures = new List<WEAPON_TYPE>();
		}

		bool IPipelineElement.IsComplete
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IPipelineElement.Connect(IPipelineElement next)
		{
			throw new NotImplementedException();
		}

		void IPipelineElement.Process()
		{
			_GetTreasure();

			var target = _DataVisitor.GetTreasures(TreasureKind.KIND.RANDOM);

			target.Clear();
			target.AddRange(_GotTreasures);
		}

		private void _GetTreasure()
		{
			var spec = _DataVisitor.Farm.FindDataRoot(_DataVisitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.SPEC);

			var list = new List<WEAPON_TYPE>();

			var randomWeapons = new SpecialWeaponRateTable().WeaponRates;

			// 計算魚掉那個寶
			foreach(var t in randomWeapons)
			{
				var rate = randomWeapons.Find(x => x.WeaponType == t.WeaponType)
										.Rate;

				long gate = (0x0FFFFFFF / rate / randomWeapons.Count) * spec.Buffer.Rate;

				gate = gate / 1000;

				if(spec.TempValueNode.HiLoRate >= 0)
				{
					gate *= 2;
				}

				if(spec.TempValueNode.HiLoRate < -200)
				{
					gate /= 2;
				}

				var randomValue = _DataVisitor.FindIRandom(RandomData.RULE.CHECK_TREASURE, 0)
											.NextInt(0, 0x10000000);
				if(randomValue >= gate)
				{
					continue;
				}

				// 在這隻魚身上得到的道具
				list.Add(t.WeaponType);
			}

			_OrderByWeapon(list);
		}

		private void _OrderByWeapon(IEnumerable<WEAPON_TYPE> list)
		{
			var weaponrandomValue = _DataVisitor.FindIRandom(RandomData.RULE.CHECK_TREASURE, 1)
												.NextFloat(0, 1);

			var randomWeapon = list.OrderBy(x => weaponrandomValue)
									.FirstOrDefault();

			if(randomWeapon != WEAPON_TYPE.INVALID)
			{
				_GotTreasures.Add(randomWeapon);
			}
		}
	}
}
