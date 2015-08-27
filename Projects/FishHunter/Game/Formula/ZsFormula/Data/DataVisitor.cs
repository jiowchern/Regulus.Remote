using System;
using System.Collections.Generic;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Data
{
    public class DataVisitor
    {
        public class RandomData
        {
            public enum RULE
            {
                ADJUSTMENT_PLAYER_PHASE,

                CHECK_TREASURE,

                DEATH,

                ODDS
            }

            public RULE RandomType { get; set; }

            public int[] RandomValue { get; set; }
        }

        public FormulaPlayerRecord PlayerRecord { get; private set; }

        public FishFarmData Farm { get; private set; }

        public FarmBuffer.BUFFER_BLOCK FocusBufferBlock { get; set; }

        public List<WEAPON_TYPE> GotTreasures { get; private set; }

        public List<RandomData> RandomDatas { get; }

        public DataVisitor(FishFarmData fish_farm, FormulaPlayerRecord formula_player_record, IRandom random)
        {
            Farm = fish_farm;
            PlayerRecord = formula_player_record;

            GotTreasures = new List<WEAPON_TYPE>();

            RandomDatas = new List<RandomData>
            {
                new RandomData
                {
                    RandomType = RandomData.RULE.ADJUSTMENT_PLAYER_PHASE,
                    RandomValue = new[]
                    {
                        random.NextInt(0, 1000)
                    }
                },
                new RandomData
                {
                    RandomType = RandomData.RULE.CHECK_TREASURE,
                    RandomValue = new[]
                    {
                        random.NextInt(0, 0x10000000),
                        random.NextInt(0, 3)
                    }
                },

                new RandomData
                {
                    RandomType = RandomData.RULE.DEATH,
                    RandomValue = new[]
                    {
                        random.NextInt(0, 0x10000000),
                        random.NextInt(0, 0x10000000),
                    }
                },
                new RandomData
                {
                    RandomType = RandomData.RULE.ODDS,
                    RandomValue = new[]
                    {
                        random.NextInt(0, 1000),
                        random.NextInt(0, 1000),
                        random.NextInt(0, 1000),
                        random.NextInt(0, 1000),
                        random.NextInt(0, 1000),
                    }
                }
            };
        }
    }
}
