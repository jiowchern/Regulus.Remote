using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.CustomType;
using Regulus.Extension;

namespace RegulusLibraryTest
{
    [TestClass]
    public class TypeTest
    {
        [TestMethod]
        public void TestRectLeftToCenter()
        {
            Rect rect = new Rect(0,1,1,1);
            var result = rect.LeftToCenter();
            Assert.AreEqual(-0.5f, result.X);
            Assert.AreEqual(0.5f, result.Y);

            Assert.AreEqual(1, result.Width);
            Assert.AreEqual(1, result.Height);
        }
         [TestMethod]
        public void TestRectCenterToLeft()
        {
            Rect rect = new Rect(0, 1, 1, 1);
            var result = rect.CenterToLeft();
            Assert.AreEqual(0.5f, result.X);
            Assert.AreEqual(1.5f, result.Y);

            Assert.AreEqual(1, result.Width);
            Assert.AreEqual(1, result.Height);
        }

        [TestMethod]
        public void TestVector2sToRect()
        {
            Vector2[] vector2s = {
                new Vector2(0,0), 
                new Vector2(2,0), 
                new Vector2(2,1), 
                new Vector2(0,1),                 
            };

            var rect = vector2s.ToRect();


            Assert.AreEqual(0, rect.X);
            Assert.AreEqual(0, rect.Y);

            Assert.AreEqual(2, rect.Width);
            Assert.AreEqual(1, rect.Height);
        }


        [TestMethod]
        public void TestPolygonCollision()
        {
            Polygon a = new Polygon();
            Polygon b = new Polygon();
            Polygon c = new Polygon();


            a.Points.Add(new Vector2(0, 0));
            a.Points.Add(new Vector2(1, 0));
            a.Points.Add(new Vector2(1, 1));
            a.Points.Add(new Vector2(0, 1));

            b.Points.AddRange(a.Points);            
            b.Offset(0.9f,0);
            b.BuildEdges();

            c.Points.AddRange(b.Points);
            c.Offset(0.9f, 0);
            c.BuildEdges();


            var resultA = Polygon.Collision(a, b, new Vector2());
            var resultB = Polygon.Collision(b, c, new Vector2());
            var resultC = Polygon.Collision(a, c, new Vector2());


            Assert.AreEqual(true , resultA.Intersect );
            Assert.AreEqual(true, resultB.Intersect);
            Assert.AreEqual(false, resultC.Intersect);

        }
    }
}
