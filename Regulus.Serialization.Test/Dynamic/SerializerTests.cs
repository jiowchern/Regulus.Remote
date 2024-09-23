using NUnit.Framework;
using Regulus.Serialization.Tests;
using System;
using System.Linq;

namespace Regulus.Serialization.Dynamic.Tests
{

    public class SerializerTests
    {
        [NUnit.Framework.Test]
        public void TestSerializerInt()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer(12345).ToArray();
            int val = (int)ser.BufferToObject(buf);
            Assert.AreEqual(12345, val);
        }

        [NUnit.Framework.Test]
        public void TestSerializerString()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer("12345").ToArray();
            string val = (string)ser.BufferToObject(buf);
            Assert.AreEqual("12345", val);
        }


        [NUnit.Framework.Test]
        public void TestSerializerNull()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer(null).ToArray();
            object val = ser.BufferToObject(buf);
            Assert.AreEqual(null, val);
        }


        [NUnit.Framework.Test]
        public void TestSerializerArray()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer(new[] { 1, 2, 3, 4, 5 }).ToArray();
            int[] val = (int[])ser.BufferToObject(buf);
            Assert.AreEqual(1, val[0]);
            Assert.AreEqual(2, val[1]);
            Assert.AreEqual(3, val[2]);
            Assert.AreEqual(4, val[3]);
            Assert.AreEqual(5, val[4]);


        }


        [NUnit.Framework.Test]
        public void TestSerializerStringArray()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer(new[] { "1", "2", "3", "4", "5" }).ToArray();
            string[] val = (string[])ser.BufferToObject(buf);
            Assert.AreEqual("1", val[0]);
            Assert.AreEqual("2", val[1]);
            Assert.AreEqual("3", val[2]);
            Assert.AreEqual("4", val[3]);
            Assert.AreEqual("5", val[4]);


        }


        [NUnit.Framework.Test]
        public void TestInherit()
        {
            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();


            TestGrandson test = new TestGrandson();
            test.Data = 100;
            TestChild testChild = test as TestChild;
            testChild.Data = 1;
            TestParent testParent = test as TestParent;
            testParent.Data = 33;

            byte[] buf = ser.ObjectToBuffer(test).ToArray();
            TestGrandson val = (TestGrandson)ser.BufferToObject(buf);
            TestParent valParent = val as TestParent;
            TestChild child = val as TestChild;


            Assert.AreEqual(100, val.Data);
            Assert.AreEqual(1, child.Data);
            Assert.AreEqual(33, valParent.Data);

        }

        /*[NUnit.Framework.Test]
        public void TestList()
        {
            var serializer = new Regulus.Serialization.Dynamic.Serializer();
            var buf = serializer.ObjectToBuffer(new System.Collections.Generic.List<int>() {1, 2, 3, 4, 5});
            var val = (System.Collections.Generic.List<int>)serializer.BufferToObject(buf);

            Assert.AreEqual(1, val[0]);
            Assert.AreEqual(2, val[1]);
            Assert.AreEqual(3, val[2]);
            Assert.AreEqual(4, val[3]);
            Assert.AreEqual(5, val[4]);
            
        }*/


        [NUnit.Framework.Test]
        public void TestArray1()
        {



            TestGrandson test = new TestGrandson();
            test.Data = 100;
            TestChild testChild = test as TestChild;
            testChild.Data = 1;
            TestParent testParent = test as TestParent;
            testParent.Data = 33;

            Serializer serializer =
                new Regulus.Serialization.Dynamic.Serializer(new CustomFinder((name) => Type.GetType(name)));
            byte[] buf = serializer.ObjectToBuffer(new TestParent[] { test, testChild, testParent }).ToArray();
            TestParent[] val = (TestParent[])serializer.BufferToObject(buf);

            Assert.AreEqual(100, (val[0] as TestGrandson).Data);
            Assert.AreEqual(1, (val[1] as TestChild).Data);
            Assert.AreEqual(33, (val[2] as TestParent).Data);


        }
        [NUnit.Framework.Test]
        public void TestClassPolytype1()
        {
            TestGrandson g = new TestGrandson();
            g.Data = 100;
            TestPoly test = new TestPoly();
            test.Parent = g;



            Serializer serializer =
                new Regulus.Serialization.Dynamic.Serializer(new CustomFinder((name) => Type.GetType(name)));
            byte[] buf = serializer.ObjectToBuffer(test).ToArray();
            TestPoly val = (TestPoly)serializer.BufferToObject(buf);
            TestGrandson valChild = val.Parent as TestGrandson;
            Assert.AreEqual(100, valChild.Data);
        }
    }
}
