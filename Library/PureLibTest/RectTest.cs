using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.CustomType;
using Regulus.Extension;

namespace RegulusLibraryTest
{
    [TestClass]
    public class RectTest
    {
        [TestMethod]
        public void TestLeftToCenter()
        {
            Rect rect = new Rect(0,1,1,1);
            var result = rect.LeftToCenter();
            Assert.AreEqual(-0.5f, result.X);
            Assert.AreEqual(0.5f, result.Y);

            Assert.AreEqual(1, result.Width);
            Assert.AreEqual(1, result.Height);
        }
         [TestMethod]
        public void TestCenterToLeft()
        {
            Rect rect = new Rect(0, 1, 1, 1);
            var result = rect.CenterToLeft();
            Assert.AreEqual(0.5f, result.X);
            Assert.AreEqual(1.5f, result.Y);

            Assert.AreEqual(1, result.Width);
            Assert.AreEqual(1, result.Height);
        }
    }
}
