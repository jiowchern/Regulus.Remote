using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItemMaker
{
    public class Maker
    {
        MaterialQualityAnalyzer _Analyzer;
        EffectBuilder _EffectBuilder;
        int _ItemId;
        public Maker(int item_id , MaterialQualityAnalyzer analyzer)
        {
            _ItemId = item_id;
            _Analyzer = analyzer;

            //_EffectBuilder = new EffectBuilder();
        }
        public Item Get(Material[] materials)
        {
            var quality = _Analyzer.GetQuality(materials);

            var effectBuilder = _EffectBuilder;
            /*var effects = effectBuilder.Get(quality);

            return _Make(_ItemId , effects);*/

            return new Item();
        }
    }
}
