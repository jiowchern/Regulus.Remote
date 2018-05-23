using NUnit.Framework;
using Regulus.Serialization.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
