using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regulus.Project.ItemMaker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Regulus.Project.ItemMaker.Tests
{
    [TestClass()]
    public class MaterialQualityAnalyzerTests
    {
        [TestMethod()]
        public void GetQualityTest()
        {
            var materials = new Material[] { new Material { Id = 1 , Count = 1},
                                            new Material { Id = 2 , Count = 7},
                                            new Material { Id = 3 , Count = 2}};
            var recipeMaterials = new Material[] { new Material { Id = 1 , Count = 5},
                                            new Material { Id = 2 , Count = 4},
                                            new Material { Id = 3 , Count = 1}};
            var recipe = new MaterialQualityAnalyzer(recipeMaterials);
            var quality = recipe.GetQuality(materials);
            var eQuality = (0.1f / 0.7f) * 0.4f + (0.7f / 0.7f) * 0.4f + (0.2f / 0.7f) * 0.4f;

            Assert.AreEqual(quality, eQuality);
            
        }

        [TestMethod()]
        public void GetQualityTest1()
        {
            var materials = new Material[] { new Material { Id = 1 , Count = 5},
                                            new Material { Id = 2 , Count = 4},
                                            new Material { Id = 3 , Count = 1}};
            var recipeMaterials = new Material[] { new Material { Id = 1 , Count = 5},
                                            new Material { Id = 2 , Count = 4},
                                            new Material { Id = 3 , Count = 1}};
            var recipe = new MaterialQualityAnalyzer(recipeMaterials);
            var quality = recipe.GetQuality(materials);
            var eQuality = 1.0f;

            Assert.AreEqual(quality, eQuality);
        }
    }
}
