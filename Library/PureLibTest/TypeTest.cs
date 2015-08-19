using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Utility;

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
        public void PolygonClone()
        {
            Polygon a = new Polygon();
            a.SetPoints( new[]{
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 1), 
                new Vector2(0, 1),                 
            });

            var b = a.Clone();
            Assert.AreEqual( 0 , b.Points[0].X );
            Assert.AreEqual(2, b.Points[1].X);
            Assert.AreEqual(1, b.Points[3].Y);
        }

        [TestMethod]
        public void TestPolygonCollision()
        {
            Polygon a = new Polygon();
            Polygon b = new Polygon();
            Polygon c = new Polygon();


            a.SetPoints(new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            });            

            b.SetPoints(a.Points);            
            b.Offset(0.9f,0);
            

            c.SetPoints(b.Points);
            c.Offset(0.9f, 0);            


            var resultA = Polygon.Collision(a, b, new Vector2());
            var resultB = Polygon.Collision(b, c, new Vector2());
            var resultC = Polygon.Collision(a, c, new Vector2());


            Assert.AreEqual(true , resultA.Intersect );
            Assert.AreEqual(true, resultB.Intersect);
            Assert.AreEqual(false, resultC.Intersect);

        }
        [TestMethod]
        public void TestProtobufEnum()
        {
            PROTOBUFENUM e = PROTOBUFENUM.AAA2;

            var obj = Regulus.TypeHelper.Serializer<PROTOBUFENUM>(e);
            var e2 = Regulus.TypeHelper.Deserialize<PROTOBUFENUM>(obj);

            Assert.AreEqual(e2, PROTOBUFENUM.AAA2);
        }        
        enum PROTOBUFENUM
        {
            AAA1,
            AAA2,
            AAA3 = AAA2,
            AAA4,
            AAA5,
            AAA6 = 100,
            AAA7,
            AAA8,
            AAA9,

        }
        [TestMethod]
        public void TestEnumCount()
        {
            int count=0;
            
            foreach (var e in EnumHelper.GetEnums<PROTOBUFENUM>())
            {
                count++;
            }

            Assert.AreEqual(9,count);
        }

        

    }
}
