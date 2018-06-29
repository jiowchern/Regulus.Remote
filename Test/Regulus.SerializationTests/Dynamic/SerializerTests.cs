using NUnit.Framework;
using Regulus.Serialization.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute.Extensions;
using Regulus.Serialization.Tests;

namespace Regulus.Serialization.Dynamic.Tests
{

    public class SerializerTests
    {
        [NUnit.Framework.Test()]
        public void TestSerializerInt()
        {

            var ser = new Regulus.Serialization.Dynamic.Serializer();

            var buf = ser.ObjectToBuffer(12345);
            var val = (int) ser.BufferToObject(buf);
            Assert.AreEqual(12345, val);
        }

        [NUnit.Framework.Test()]
        public void TestSerializerString()
        {

            var ser = new Regulus.Serialization.Dynamic.Serializer();

            var buf = ser.ObjectToBuffer("12345");
            var val = (string) ser.BufferToObject(buf);
            Assert.AreEqual("12345", val);
        }


        [NUnit.Framework.Test()]
        public void TestSerializerNull()
        {

            var ser = new Regulus.Serialization.Dynamic.Serializer();

            var buf = ser.ObjectToBuffer(null);
            var val = ser.BufferToObject(buf);
            Assert.AreEqual(null, val);
        }


        [NUnit.Framework.Test()]
        public void TestSerializerArray()
        {

            var ser = new Regulus.Serialization.Dynamic.Serializer();

            var buf = ser.ObjectToBuffer(new [] {1,2,3,4,5});
            var val = (int[])ser.BufferToObject(buf);
            Assert.AreEqual(1, val[0]);
            Assert.AreEqual(2, val[1]);
            Assert.AreEqual(3, val[2]);
            Assert.AreEqual(4, val[3]);
            Assert.AreEqual(5, val[4]);

            
        }


        [NUnit.Framework.Test()]
        public void TestSerializerStringArray()
        {

            var ser = new Regulus.Serialization.Dynamic.Serializer();

            var buf = ser.ObjectToBuffer(new[] { "1", "2", "3", "4", "5" });
            var val = (string[])ser.BufferToObject(buf);
            Assert.AreEqual("1", val[0]);
            Assert.AreEqual("2", val[1]);
            Assert.AreEqual("3", val[2]);
            Assert.AreEqual("4", val[3]);
            Assert.AreEqual("5", val[4]);


        }


        [NUnit.Framework.Test()]
        public void TestInherit()
        {
            var ser = new Regulus.Serialization.Dynamic.Serializer();


            var test = new TestGrandson();
            test.Data = 100;
            var testChild = test as TestChild;
            testChild.Data = 1;
            var testParent = test as TestParent;
            testParent.Data = 33;

            var buf = ser.ObjectToBuffer(test);
            var val = (TestGrandson)ser.BufferToObject(buf);
            var valParent = val as TestParent;
            var child = val as TestChild;


            Assert.AreEqual(100, val.Data);
            Assert.AreEqual(1, child.Data);
            Assert.AreEqual(33, valParent.Data);

        }

        [NUnit.Framework.Test()]
        public void TestList()
        {
            /*var serializer = new Regulus.Serialization.Dynamic.Serializer();
            var buf = serializer.ObjectToBuffer(new System.Collections.Generic.List<int>() {1, 2, 3, 4, 5});
            var val = (System.Collections.Generic.List<int>)serializer.BufferToObject(buf);

            Assert.AreEqual(1, val[0]);
            Assert.AreEqual(2, val[1]);
            Assert.AreEqual(3, val[2]);
            Assert.AreEqual(4, val[3]);
            Assert.AreEqual(5, val[4]);*/
            
        }
    }
}
