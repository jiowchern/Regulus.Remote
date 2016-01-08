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
				if(_IsSpecialFish(fishData))
				{
					fishData.Multiple = 1;
					continue;
				}

				if(_IsSpecialFish2(fishData))
				{
					fishData.Multiple = 1;
					continue;
				}

				if(_IsSmallFish(fishData))
				{
					fishData.Multiple = 1;
					continue;
				}

				if(_IsFreeze(fishData))
				{
					fishData.Multiple = 2;
					continue;
				}

				if(!_CheckExceptionsFishType(fishData))
				{
					fishData.Multiple = 1;
					continue;
				}

				if(!_CheckStageRate())
				{
					fishData.Multiple = 1;
					continue;
				}

				fishData.Multiple = _GetResult(fishData);
			}
		}

		private static bool _IsSpecialFish2(RequsetFishData fish_data)
		{
			return fish_data.FishType == FISH_TYPE.SPECIAL_EAT_FISH || fish_data.FishType == FISH_TYPE.SPECIAL_EAT_FISH_CRAZY;
		}

		/// <summary>
		///     特殊魚不翻倍
		/// </summary>
		private static bool _IsSpecialFish(RequsetFishData fish_data)
		{
			return fish_data.FishType >= FISH_TYPE.SPECIAL_SCREEN_BOMB && fish_data.FishType <= FISH_TYPE.SPECIAL_BIG_OCTOPUS_BOMB;
		}

		/// <summary>
		///     小倍數魚不翻倍
		/// </summary>
		private static bool _IsSmallFish(RequsetFishData fish_data)
		{
			return fish_data.FishOdds < 10;
		}

		/// <summary>
		///     其它魚只要冰凍必翻2倍
		/// </summary>
		private static bool _IsFreeze(RequsetFishData fish_data)
		{
			return fish_data.FishStatus == FISH_STATUS.FREEZE;
		}

		/// <summary>
		///     例外魚種
		/// </summary>
		private bool _CheckExceptionsFishType(RequsetFishData fish_data)
		{
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

		/// <summary>
		///     比對漁場資料
		/// </summary>
		private bool _CheckStageRate()
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

		/// <summary>
		///     計算翻倍結果
		/// </summary>
		private int _GetResult(RequsetFishData fish_data)
		{
			var randNumber = _Visitor.FindIRandom(RandomData.RULE.ODDS, 4)
									.NextInt(0, 1000);

			var oddsValue = new OddsTable().CheckRule(randNumber);

			return _IsExceptions(fish_data, oddsValue) ? 1 : oddsValue;
		}

		/// <summary>
		///     例外狀況
		/// </summary>
		private static bool _IsExceptions(RequsetFishData fish_data, int odds)
		{
			return fish_data.FishOdds < 50 && odds == 10;
		}
	}
}
