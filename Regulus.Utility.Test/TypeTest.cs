using Regulus.Extension;
using Regulus.Utility;
using System.IO;
using System.Xml.Serialization;

namespace RegulusLibraryTest
{

    public class TypeTest
    {


        [NUnit.Framework.Test()]
        public void TestRectLeftToCenter()
        {
            Rect rect = new Rect(0, 1, 1, 1);
            Rect result = rect.LeftToCenter();
            NUnit.Framework.Assert.AreEqual(-0.5f, result.X);
            NUnit.Framework.Assert.AreEqual(0.5f, result.Y);

            NUnit.Framework.Assert.AreEqual(1, result.Width);
            NUnit.Framework.Assert.AreEqual(1, result.Height);
        }
        [NUnit.Framework.Test()]
        public void TestRectCenterToLeft()
        {
            Rect rect = new Rect(0, 1, 1, 1);
            Rect result = rect.CenterToLeft();
            NUnit.Framework.Assert.AreEqual(0.5f, result.X);
            NUnit.Framework.Assert.AreEqual(1.5f, result.Y);

            NUnit.Framework.Assert.AreEqual(1, result.Width);
            NUnit.Framework.Assert.AreEqual(1, result.Height);
        }

        [NUnit.Framework.Test()]
        public void TestVector2sToRect()
        {
            Vector2[] vector2s = {
                new Vector2(0,0),
                new Vector2(2,0),
                new Vector2(2,1),
                new Vector2(0,1),
            };

            Rect rect = vector2s.ToRect();


            NUnit.Framework.Assert.AreEqual(0, rect.X);
            NUnit.Framework.Assert.AreEqual(0, rect.Y);

            NUnit.Framework.Assert.AreEqual(2, rect.Width);
            NUnit.Framework.Assert.AreEqual(1, rect.Height);
        }

        [NUnit.Framework.Test()]
        public void PolygonClone()
        {
            Polygon a = new Polygon();
            a.SetPoints(new[]{
                new Vector2(0, 0),
                new Vector2(2, 0),
                new Vector2(2, 1),
                new Vector2(0, 1),
            });

            Polygon b = a.Clone();
            NUnit.Framework.Assert.AreEqual(0, b.Points[0].X);
            NUnit.Framework.Assert.AreEqual(2, b.Points[1].X);
            NUnit.Framework.Assert.AreEqual(1, b.Points[3].Y);
        }

        [NUnit.Framework.Test()]
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
            b.Offset(0.9f, 0);


            c.SetPoints(b.Points);
            c.Offset(0.9f, 0);


            Polygon.CollisionResult resultA = Polygon.Collision(a, b, new Vector2());
            Polygon.CollisionResult resultB = Polygon.Collision(b, c, new Vector2());
            Polygon.CollisionResult resultC = Polygon.Collision(a, c, new Vector2());


            NUnit.Framework.Assert.AreEqual(true, resultA.Intersect);
            NUnit.Framework.Assert.AreEqual(true, resultB.Intersect);
            NUnit.Framework.Assert.AreEqual(false, resultC.Intersect);

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
        [NUnit.Framework.Test()]
        public void TestEnumCount()
        {
            int count = 0;

            foreach (PROTOBUFENUM e in EnumHelper.GetEnums<PROTOBUFENUM>())
            {
                count++;
            }

            NUnit.Framework.Assert.AreEqual(9, count);
        }

        [NUnit.Framework.Test()]
        public void TestPolygonsXMLSerializ()
        {
            Polygon polygon1 = new Polygon();
            polygon1.SetPoints(new[]{
                    new Vector2(0,0),
                    new Vector2(1,0),
                    new Vector2(1,1),
                    new Vector2(0,1)});
            Polygon polygon2 = new Polygon();
            polygon2.SetPoints(new[]{
                    new Vector2(2,0),
                    new Vector2(1,0),
                    new Vector2(1,1),
                    new Vector2(0,1)});
            Polygon[] polygons1 = new Polygon[]
            {
                polygon1,
                polygon2
            };
            string xml = "";
            using (StringWriter stream = new StringWriter())
            {
                XmlSerializer x = new XmlSerializer(typeof(Polygon[]));
                x.Serialize(stream, polygons1);
                xml = stream.ToString();
            }
            Polygon[] polygons2;
            using (StringReader stream = new StringReader(xml))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Polygon[]));
                polygons2 = (Polygon[])ser.Deserialize(stream);
            }

            NUnit.Framework.Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(polygons2, polygons1));
        }

        [NUnit.Framework.Test()]
        public void TestPolygonXMLSerializ()
        {
            Polygon polygon1 = new Polygon();
            polygon1.SetPoints(new[]{
                    new Vector2(0,0),
                    new Vector2(1,0),
                    new Vector2(1,1),
                    new Vector2(0,1)});
            string xml = "";
            using (StringWriter stream = new StringWriter())
            {
                XmlSerializer x = new XmlSerializer(typeof(Polygon));
                x.Serialize(stream, polygon1);
                xml = stream.ToString();
            }
            Polygon polygon2;
            using (StringReader stream = new StringReader(xml))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Polygon));
                polygon2 = (Polygon)ser.Deserialize(stream);
            }


            NUnit.Framework.Assert.AreEqual(polygon2.Points[0], polygon1.Points[0]);
        }

    }
}
