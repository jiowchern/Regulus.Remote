// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OddsRuler.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   00以上为特殊鱼（死亡后，有爆炸效果的），直接变成特殊武器
//   海绵宝宝x80、电鳗x150、贪食蛇x120、铁球x200、小章鱼x200、大章鱼xN（必死）
//   翻倍規則檢查,
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;


using Regulus.Utility;

using VGame.Project.FishHunter.Common.Datas.FishStage;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     00以上为特殊鱼（死亡后，有爆炸效果的），直接变成特殊武器
	///     海绵宝宝x80、电鳗x150、贪食蛇x120、铁球x200、小章鱼x200、大章鱼xN（必死）
	///     翻倍規則檢查,
	/// </summary>
	public class OddsRuler
	{
		private readonly StageBuffer _BufferData;

		private readonly FishDataTable.Data _FishData;

		public OddsRuler(FishDataTable.Data fish_data, StageBuffer buffer_data)
		{
			_FishData = fish_data;
			_BufferData = buffer_data;
		}

		public int RuleResult()
		{
			_CheckFishTypeToOddsRule(_FishData);

			_CheckStageBufferToOddsRule(_BufferData);

			var wup = _CheckMultipleTableToOddsRule();

			return _CheckFishMultipleToOddsRule(_FishData, wup);
		}

		private bool _CheckFishTypeToOddsRule(FishDataTable.Data fish_data)
		{
			if ((int)fish_data.FishType >= 100)
			{
				return false; // 特殊鱼 不翻倍
			}

			if (fish_data.Odds < 10)
			{
				return false; // 小鱼 不翻倍 
			}

			if (fish_data.FishType == FishDataTable.Data.FISH_TYPE.DEF_100_A)
			{
				var randNumber = Random.Instance.NextInt(0, 1000) % 1000;
				if (randNumber < 500)
				{
					return false;
				}
			}

			if (fish_data.FishType == FishDataTable.Data.FISH_TYPE.DEF_200_A)
			{
				var randNumber = Random.Instance.NextInt(0, 1000) % 1000;
				if (randNumber < 750)
				{
					return false;
				}
			}

			if (fish_data.FishType == FishDataTable.Data.FISH_TYPE.DEF_400_A)
			{
				var randNumber = Random.Instance.NextInt(0, 1000) % 1000;
				if (randNumber < 875)
				{
					return false;
				}
			}

			return true;
		}

		private bool _CheckStageBufferToOddsRule(StageBuffer data)
		{
			var natrue = new NatureBufferChancesTable(null);

			var hiLoRate = data.BufferTempValue.HiLoRate;

			var rand = Random.Instance.NextInt(0, 1000);

			if (hiLoRate <= natrue.FindValue(-3))
			{
				if (rand < 750)
				{
					// 有75% 不翻倍
					return false;
				}
			}
			else if (hiLoRate <= natrue.FindValue(-2))
			{
				if (rand < 500)
				{
					// 有50% 不翻倍
					return false;
				}
			}
			else if (hiLoRate <= natrue.FindValue(-1))
			{
				if (rand < 250)
				{
					// 有25% 不翻倍
					return false;
				}
			}
			else if (hiLoRate <= natrue.FindValue(0))
			{
				if (rand < 100)
				{
					// 有10% 不翻倍
					return false;
				}
			}

			return true;
		}

		private int _CheckMultipleTableToOddsRule()
		{
			var rand = Random.Instance.NextInt(0, 1000);
			var multiple = new MultipleTable(null);

			var lastKey = 0;

			foreach (var data in multiple.Datas.Values)
			{
				lastKey = data.Multiple;
				if (rand < data.Value)
				{
					break;
				}

				rand -= data.Value;
			}

			return multiple.Datas.Last(x => x.Value.Multiple != lastKey).Value.Value;
		}

		private int _CheckFishMultipleToOddsRule(FishDataTable.Data fish_data, int wup)
		{
			if ((fish_data.Odds < 50) && (wup == 10))
			{
				wup = 1;
			}

			return wup;
		}
	}
}