
using System;
using System.Diagnostics;
using System.Linq;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
    /// <summary>
    ///     翻倍規則檢查
    /// </summary>
    public class OddsRuler
    {
        private readonly FarmBuffer _BufferData;

        private readonly RequsetFishData _FishData;

        private readonly DataVisitor _Visitor;

        private readonly int[] _Randoms;

        public OddsRuler(DataVisitor visitor, RequsetFishData fish_data, FarmBuffer buffer_data)
        {
            _Visitor = visitor;
            _FishData = fish_data;
            _BufferData = buffer_data;

            _Randoms =
                _Visitor.RandomDatas.Find(x => x.RandomType == DataVisitor.RandomData.RULE.ODDS).RandomValue;
        }

	    public int RuleResult()
        {
            if(_CheckIsFreeze(_FishData))
            {
                return 2;
            }

            if (!_CheckFishTypeToOddsRule(_FishData))
            {
                return 1;
            }

            if(!_CheckStageBufferToOddsRule(_BufferData))
            {
                return 1;
            }

            return _CheckMultipleTableToOddsRule();
        }

	    private bool _CheckIsFreeze(RequsetFishData fish_data)
	    {
		    if (fish_data.FishType >= FISH_TYPE.SPECIAL_SCREEN_BOMB
				&& fish_data.FishType <= FISH_TYPE.SPECIAL_BIG_OCTOPUS_BOMB)
		    {
			    return false; // 特殊鱼 不翻倍
		    }

		    // 其它魚只要冰凍必翻2倍
		    return _FishData.FishStatus == FISH_STATUS.FREEZE;
	    }

	    private bool _CheckFishTypeToOddsRule(RequsetFishData fish_data)
        {
            if(fish_data.FishType >= FISH_TYPE.SPECIAL_SCREEN_BOMB
               && fish_data.FishType <= FISH_TYPE.SPECIAL_BIG_OCTOPUS_BOMB)
            {
                return false; // 特殊鱼 不翻倍
            }

            if(fish_data.FishOdds < 10)
            {
                return false; // 小鱼 不翻倍 
            }

            if(fish_data.FishType == FISH_TYPE.BLUE_WHALE)
            {
                var randNumber = _Randoms[0];
                if(randNumber < 500)
                {
                    return false; // 藍鯨 50%不翻倍 
                }
            }

            if(fish_data.FishType == FISH_TYPE.RED_WHALE)
            {
                var randNumber = _Randoms[1];
                if(randNumber < 750)
                {
                    return false; // 藍鯨 75%不翻倍
                }
            }

            if(fish_data.FishType == FISH_TYPE.GOLDEN_WHALE)
            {
                var randNumber = _Randoms[2];
                if(randNumber < 875)
                {
                    return false; // 金鯨 87%不翻倍
                }
            }

            return true;
        }

        private bool _CheckStageBufferToOddsRule(FarmBuffer data)
        {
            var natrue = new NatureBufferChancesTable().Get().ToDictionary(x => x.Key);

            var hiLoRate = data.BufferTempValue.HiLoRate;

            var rand = _Randoms[3];

            if(hiLoRate <= natrue[-3].Value)
            {
                if(rand < 750)
                {
                    // 有75% 不翻倍
                    return false;
                }
            }
            else if(hiLoRate <= natrue[-2].Value)
            {
                if(rand < 500)
                {
                    // 有50% 不翻倍
                    return false;
                }
            }
            else if(hiLoRate <= natrue[-1].Value)
            {
                if(rand < 250)
                {
                    // 有25% 不翻倍
                    return false;
                }
            }
            else if(hiLoRate <= natrue[0].Value)
            {
                if(rand < 100)
                {
                    // 有10% 不翻倍
                    return false;
                }
            }

            return true;
        }

        private int _CheckMultipleTableToOddsRule()
        {
            var rand = _Randoms[4];

            var oddsTable = new OddsTable().Datas;

			OddsTable.Data temp = null;

            foreach (var d in oddsTable)
            {
                temp = d;
                if(rand < d.Number)
                {
                    break;
                }

                rand -= d.Number;
            }

            Debug.Assert(temp != null, "temp != null");

            return _CheckFishToOddsRule(_FishData, temp.Odds)
                       ? temp.Odds
                       : 1;
        }

        private bool _CheckFishToOddsRule(RequsetFishData fish_data, int odds)
        {
            return (fish_data.FishOdds >= 50) || (odds != 10);
        }
    }
}
