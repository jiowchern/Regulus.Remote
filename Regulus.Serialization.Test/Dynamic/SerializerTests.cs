using Xunit;
using Regulus.Serialization.Tests;
using System;

namespace Regulus.Serialization.Dynamic.Tests
{

    public class SerializerTests
    {
        [Xunit.Fact]
        public void TestSerializerInt()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer(12345);
            int val = (int)ser.BufferToObject(buf);
            Assert.Equal(12345, val);
        }

        [Xunit.Fact]
        public void TestSerializerString()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer("12345");
            string val = (string)ser.BufferToObject(buf);
            Assert.Equal("12345", val);
        }


        [Xunit.Fact]
        public void TestSerializerNull()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer(null);
            object val = ser.BufferToObject(buf);
            Assert.Equal(null, val);
        }


        [Xunit.Fact]
        public void TestSerializerArray()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer(new[] { 1, 2, 3, 4, 5 });
            int[] val = (int[])ser.BufferToObject(buf);
            Assert.Equal(1, val[0]);
            Assert.Equal(2, val[1]);
            Assert.Equal(3, val[2]);
            Assert.Equal(4, val[3]);
            Assert.Equal(5, val[4]);


        }


        [Xunit.Fact]
        public void TestSerializerStringArray()
        {

            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();

            byte[] buf = ser.ObjectToBuffer(new[] { "1", "2", "3", "4", "5" });
            string[] val = (string[])ser.BufferToObject(buf);
            Assert.Equal("1", val[0]);
            Assert.Equal("2", val[1]);
            Assert.Equal("3", val[2]);
            Assert.Equal("4", val[3]);
            Assert.Equal("5", val[4]);


        }


        [Xunit.Fact]
        public void TestInherit()
        {
            Serializer ser = new Regulus.Serialization.Dynamic.Serializer();


            TestGrandson test = new TestGrandson();
            test.Data = 100;
            TestChild testChild = test as TestChild;
            testChild.Data = 1;
            TestParent testParent = test as TestParent;
            testParent.Data = 33;

            byte[] buf = ser.ObjectToBuffer(test);
            TestGrandson val = (TestGrandson)ser.BufferToObject(buf);
            TestParent valParent = val as TestParent;
            TestChild child = val as TestChild;


            Assert.Equal(100, val.Data);
            Assert.Equal(1, child.Data);
            Assert.Equal(33, valParent.Data);

        }

        /*[Xunit.Fact]
        public void TestList()
        {
            var serializer = new Regulus.Serialization.Dynamic.Serializer();
            var buf = serializer.ObjectToBuffer(new System.Collections.Generic.List<int>() {1, 2, 3, 4, 5});
            var val = (System.Collections.Generic.List<int>)serializer.BufferToObject(buf);

            Assert.Equal(1, val[0]);
            Assert.Equal(2, val[1]);
            Assert.Equal(3, val[2]);
            Assert.Equal(4, val[3]);
            Assert.Equal(5, val[4]);
            
        }*/


        [Xunit.Fact]
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
            byte[] buf = serializer.ObjectToBuffer(new TestParent[] { test, testChild, testParent });
            TestParent[] val = (TestParent[])serializer.BufferToObject(buf);

            Assert.Equal(100, (val[0] as TestGrandson).Data);
            Assert.Equal(1, (val[1] as TestChild).Data);
            Assert.Equal(33, (val[2] as TestParent).Data);


        }
        [Xunit.Fact]
        public void TestClassPolytype1()
        {
            TestGrandson g = new TestGrandson();
            g.Data = 100;
            TestPoly test = new TestPoly();
            test.Parent = g;



            Serializer serializer =
                new Regulus.Serialization.Dynamic.Serializer(new CustomFinder((name) => Type.GetType(name)));
            byte[] buf = serializer.ObjectToBuffer(test);
            TestPoly val = (TestPoly)serializer.BufferToObject(buf);
            TestGrandson valChild = val.Parent as TestGrandson;
            Assert.Equal(100, valChild.Data);
        }
    }
}
