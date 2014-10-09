using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItemMaker
{
    public class MaterialQualityAnalyzer
    {
        struct MaterialInfomation
        {
            public float Proportion;
            public Material Material;
        }

        private MaterialInfomation[] _Materials;
        int _Amount;

        public MaterialQualityAnalyzer(Material[] recipe_materials)
        {
            _Amount = (from m in recipe_materials select m.Count).Sum();
            _Materials = (from m in recipe_materials
                              let p = m.Count / (float)_Amount
                              select new MaterialInfomation { Proportion = p, Material = m }).ToArray();
        }

        public float GetQuality(Material[] materials)
        {
            var amount = _Amount;
            var proportions = _Materials;
            

            var amount2 = (from m in materials select m.Count).Sum();


            var proportions2 = from m in materials
                               let p = m.Count / (float)amount2
                               orderby m.Count descending
                               select new { Proportion = p, Materials = m };

            var benchmark = proportions2.FirstOrDefault();
            var baseQuality = (from p in proportions where benchmark.Materials.Id == p.Material.Id select p.Proportion).SingleOrDefault();

            float quality = 0.0f;
            foreach(var proportion in proportions2)
            {
                quality += (proportion.Proportion / benchmark.Proportion) * baseQuality;
            }

            return quality;
        }
    }
}
