using System;
using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	/// <summary>
	///     翻倍規則檢查
	/// </summary>
	public class Odds : IPipelineElement
	{
		private readonly HitRequest _HitRequest;

		private readonly DataVisitor _Visitor;

		public Odds(DataVisitor visitor, HitRequest hit_request)
		{
			_Visitor = visitor;
			_HitRequest = hit_request;
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
			foreach(var fishData in _HitRequest.FishDatas)
			{
				if(_CheckIsFreeze(fishData))
				{
					fishData.IsDoubled = true;
					continue;
				}

				if(!_CheckFishTypeToOddsRule(fishData))
				{
					fishData.IsDoubled = false;
					continue;
				}

				if(!_CheckStageBufferToOddsRule())
				{
					fishData.IsDoubled = false;
					continue;
				}

				if(_CheckMultipleTableToOddsRule(fishData))
				{
					fishData.IsDoubled = true;
				}
			}
		}

		private bool _CheckIsFreeze(RequsetFishData fish_data)
		{
			if(fish_data.FishType >= FISH_TYPE.SPECIAL_SCREEN_BOMB && fish_data.FishType <= FISH_TYPE.SPECIAL_BIG_OCTOPUS_BOMB)
			{
				return false; // 特殊鱼 不翻倍
			}

			// 其它魚只要冰凍必翻2倍
			return fish_data.FishStatus == FISH_STATUS.FREEZE;
		}

		private bool _CheckFishTypeToOddsRule(RequsetFishData fish_data)
		{
			if(fish_data.FishType >= FISH_TYPE.SPECIAL_SCREEN_BOMB && fish_data.FishType <= FISH_TYPE.SPECIAL_BIG_OCTOPUS_BOMB)
			{
				return false; // 特殊鱼 不翻倍
			}

			if(fish_data.FishOdds < 10)
			{
				return false; // 小鱼 不翻倍 
			}

			if(fish_data.FishType == FISH_TYPE.BLUE_WHALE)
			{
				var randNumber = _Visitor.FindIRandom(RandomData.RULE.ODDS, 0)
										.NextInt(0, 1000);
				if(randNumber < 500)
				{
					return false; // 藍鯨 50%不翻倍 
				}
			}

			if(fish_data.FishType == FISH_TYPE.RED_WHALE)
			{
				var randNumber = _Visitor.FindIRandom(RandomData.RULE.ODDS, 1)
										.NextInt(0, 1000);
				if(randNumber < 750)
				{
					return false; // 藍鯨 75%不翻倍
				}
			}

			if(fish_data.FishType == FISH_TYPE.GOLDEN_WHALE)
			{
				var randNumber = _Visitor.FindIRandom(RandomData.RULE.ODDS, 2)
										.NextInt(0, 1000);
				if(randNumber < 875)
				{
					return false; // 金鯨 87%不翻倍
				}
			}

			return true;
		}

		private bool _CheckStageBufferToOddsRule()
		{
			var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);
			var natrue = new NatureBufferChancesTable().Get()
														.ToDictionary(x => x.Key);

			var hiLoRate = normal.TempValueNode.HiLoRate;

			var randNumber = _Visitor.FindIRandom(RandomData.RULE.ODDS, 3)
									.NextInt(0, 1000);

			if(hiLoRate <= natrue[-3].Value)
			{
				if(randNumber < 750)
				{
					// 有75% 不翻倍
					return false;
				}
			}
			else if(hiLoRate <= natrue[-2].Value)
			{
				if(randNumber < 500)
				{
					// 有50% 不翻倍
					return false;
				}
			}
			else if(hiLoRate <= natrue[-1].Value)
			{
				if(randNumber < 250)
				{
					// 有25% 不翻倍
					return false;
				}
			}
			else if(hiLoRate <= natrue[0].Value)
			{
				if(randNumber < 100)
				{
					// 有10% 不翻倍
					return false;
				}
			}

			return true;
		}

		private bool _CheckMultipleTableToOddsRule(RequsetFishData fish_data)
		{
			var randNumber = _Visitor.FindIRandom(RandomData.RULE.ODDS, 4)
									.NextInt(0, 1000);

			var oddsValue = new OddsTable().CheckRule(randNumber);

			return _CheckFishToOddsRule(fish_data, oddsValue);
		}

		private bool _CheckFishToOddsRule(RequsetFishData fish_data, int odds)
		{
			return (fish_data.FishOdds >= 50) || (odds != 10);
		}
	}
}
