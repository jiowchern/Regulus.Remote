using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


using Regulus.Game;
using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.ZsFormula.Data;

namespace VGame.Project.FishHunter.ZsFormula.Rule
{
	/// <summary>
	///     翻倍規則檢查
	/// </summary>
	public class OddsRuler
	{
		private readonly StageBuffer _BufferData;

		private readonly RequsetFishData _FishData;

		public OddsRuler(RequsetFishData fish_data, StageBuffer buffer_data)
		{
			_FishData = fish_data;
			_BufferData = buffer_data;
		}

		public int RuleResult()
		{
			if (!_CheckFishTypeToOddsRule(_FishData))
			{
				return 1;
			}

			if (!_CheckStageBufferToOddsRule(_BufferData))
			{
				return 1;
			}

			return _CheckMultipleTableToOddsRule();
		}

		private bool _CheckFishTypeToOddsRule(RequsetFishData fish_data)
		{
			if (fish_data.FishType >= FISH_TYPE.SPECIAL_FISH_BEGIN)
			{
				return false; // 特殊鱼 不翻倍
			}

			if (fish_data.FishOdds < 10)
			{
				return false; // 小鱼 不翻倍 
			}

			if (fish_data.FishType == FISH_TYPE.BLUE_WHALE)
			{
				var randNumber = Random.Instance.NextInt(0, 1000);
				if (randNumber < 500)
				{
					return false; // 藍鯨 50%不翻倍 
				}
			}

			if (fish_data.FishType == FISH_TYPE.RED_WHALE)
			{
				var randNumber = Random.Instance.NextInt(0, 1000);
				if (randNumber < 750)
				{
					return false; // 藍鯨 75%不翻倍
				}
			}

			if (fish_data.FishType == FISH_TYPE.GOLDEN_WHALE)
			{
				var randNumber = Random.Instance.NextInt(0, 1000);
				if (randNumber < 875)
				{
					return false; // 金鯨 87%不翻倍
				}
			}

			return true;
		}

		private bool _CheckStageBufferToOddsRule(StageBuffer data)
		{
			var natrue = new NatureBufferChancesTable().Get().ToDictionary(x=>x.Key);

			var hiLoRate = data.BufferTempValue.HiLoRate;

			var rand = Random.Instance.NextInt(0, 1000);

			if (hiLoRate <= natrue[-3].Value)
			{
				if (rand < 750)
				{
					// 有75% 不翻倍
					return false;
				}
			}
			else if (hiLoRate <= natrue[-2].Value)
			{
				if (rand < 500)
				{
					// 有50% 不翻倍
					return false;
				}
			}
			else if (hiLoRate <= natrue[-1].Value)
			{
				if (rand < 250)
				{
					// 有25% 不翻倍
					return false;
				}
			}
			else if (hiLoRate <= natrue[0].Value)
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

			var oddsTable = new OddsTable().Get();

			OddsTable.Data temp = null;

			foreach (var d in oddsTable)
			{
				temp = d;
				if (rand < d.Odds)
				{
					break;
				}

				rand -= d.Odds;
			}

			Debug.Assert(temp != null, "temp != null");

			return !_CheckFishToOddsRule(_FishData, temp.Number)
				? 1
				: temp.Number;
		}

		private bool _CheckFishToOddsRule(RequsetFishData fish_data, int wup)
		{
			if ((fish_data.FishOdds < 50) && (wup == 10))
			{
				return false;
			}

			return true;
		}
	}
}