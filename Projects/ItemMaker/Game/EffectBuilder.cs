using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItemMaker
{
    public class EffectBuilder
    {

        EffectCost[] _Costs;
        public EffectBuilder(EffectCost[] costs)
        {
            _Costs = costs;
        }

        public Effect[] Get(float quality , Regulus.Utility.IRandomProvider provider)
        {
            var effects = new List<Effect>();
            var effectCosts = new List<EffectCost>(_Costs) ;
            while(quality > 0)
            {
                var cost = (from e in effectCosts
                            where e.Quality >= quality && _RollOut(e.Chances, provider.Next() )
                             orderby e.Quality descending
                             select e).FirstOrDefault();
                if (cost == null)
                    break;
                effectCosts.Remove(cost);
                quality -= cost.Quality;

                effects.Add(cost.Effect);
            }

            return effects.ToArray();
        }

        private bool _RollOut(float value1 , float value2)
        {
            return value1 <= value2;
        }
    }
}