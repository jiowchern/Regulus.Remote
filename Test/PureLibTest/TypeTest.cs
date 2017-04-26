using System;
using System.IO;
using System.Xml.Serialization;


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

        [TestMethod]
        public void TestPolygonsXMLSerializ()
        {
            var polygon1 = new Polygon();
            polygon1.SetPoints(new[]{
                    new Vector2(0,0),
                    new Vector2(1,0),
                    new Vector2(1,1),
                    new Vector2(0,1)});
            var polygon2 = new Polygon();
            polygon2.SetPoints(new[]{
                    new Vector2(2,0),
                    new Vector2(1,0),
                    new Vector2(1,1),
                    new Vector2(0,1)});
            var polygons1 = new Polygon[]
            {
                polygon1,
                polygon2
            };            
            var xml = "";
            using (var stream = new StringWriter())
            {
                var x = new XmlSerializer(typeof(Polygon[]));
                x.Serialize(stream, polygons1);
                xml = stream.ToString();
            }
            Polygon[] polygons2;
            using (var stream = new StringReader(xml))
            {
                var ser = new XmlSerializer(typeof(Polygon[]));
                polygons2 = (Polygon[])ser.Deserialize(stream);
            }

            Assert.IsTrue( Regulus.Utility.ValueHelper.DeepEqual(polygons2 , polygons1));
        }
       
        [TestMethod]
        public void TestPolygonXMLSerializ()
        {
            var polygon1 = new Polygon();
            polygon1.SetPoints(new[]{
                    new Vector2(0,0),
                    new Vector2(1,0),
                    new Vector2(1,1),
                    new Vector2(0,1)});
            var xml = "";
            using (var stream = new StringWriter())
            {
                var x = new XmlSerializer(typeof(Polygon));
                x.Serialize(stream, polygon1);
                xml= stream.ToString();
            }
            Polygon polygon2;
            using (var stream = new StringReader(xml))
            {
                var ser = new XmlSerializer(typeof(Polygon));
                polygon2=(Polygon)ser.Deserialize(stream);
            }


            Assert.AreEqual(polygon2.Points[0], polygon1.Points[0]);
        }

    }
}
