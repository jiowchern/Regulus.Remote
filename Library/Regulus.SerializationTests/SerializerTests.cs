using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Serialization.Tests
{
    [TestClass()]
    public class SerializerTests
    {
        

        


        [TestMethod()]
        public void EncodeZigZag()
        {
            var e64_1 = Serializer.ZigZag.Encode((long)1);
            var e64_2 = Serializer.ZigZag.Encode(-1L);

            var e32_1 = Serializer.ZigZag.Encode(1);
            var e32_2 = Serializer.ZigZag.Encode(-1);


            Assert.AreEqual((uint)2, e32_1);
            Assert.AreEqual((uint)1, e32_2);

            Assert.AreEqual((ulong)2, e64_1);
            Assert.AreEqual((ulong)1, e64_2);

            
        }

        [TestMethod()]
        public void DecodeZigZag()
        {
            var e64_1 = Serializer.ZigZag.Decode((ulong)2);
            var e64_2 = Serializer.ZigZag.Decode((ulong)1);

            var e32_1 = Serializer.ZigZag.Decode(2);
            var e32_2 = Serializer.ZigZag.Decode(1);


            Assert.AreEqual(1, e32_1);
            Assert.AreEqual(-1, e32_2);

            Assert.AreEqual(1, e64_1);
            Assert.AreEqual(-1, e64_2);


        }
        [TestMethod()]
        public void ZigZagTest()
        {
            var value = Serializer.ZigZag.Encode(874534L);
            var result = Serializer.ZigZag.Decode(value);
            Assert.AreEqual(874534L, result);
        }


        [TestMethod()]
        public void VarintToBufferTest()
        {

            var count = Serializer.Varint.GetByteCount(150);
            var buffer = new byte[count];
            var index = Serializer.Varint.NumberToBuffer(buffer,0 ,150);


            Assert.AreEqual(0x96, buffer[0]);
            Assert.AreEqual(0x1, buffer[1]);
            Assert.AreEqual(2, index);

        }


        [TestMethod()]
        public void VarintToNumberTest()
        {
            ulong number;
            var index = Serializer.Varint.BufferToNumber(new byte[] {0x96 ,0x1} , 0 , out number);
            Assert.AreEqual(150UL, number);
            Assert.AreEqual(2, index);
        }


        [TestMethod()]
        public void VarintTest()
        {
            var count = Serializer.Varint.GetByteCount(12671L);
            var buffer = new byte[count];
            Serializer.Varint.NumberToBuffer(buffer, 0, 12671L);

            ulong value;
            Serializer.Varint.BufferToNumber(buffer , 0 , out value);

            Assert.AreEqual(12671UL , value);
        }

        [TestMethod()]
        public void Test1()
        {
            
            var ser = new Serializer(new NumberType<ulong>(1));

            var buffer = ser.ObjectToBuffer(1UL);

            var value = ser.BufferToObject(buffer);

            Assert.AreEqual(1UL, value);
        }


        [TestMethod()]
        public void Test2()
        {
            var ser = new Serializer(new NumberType<int>(1) , new ClassType<TestClassB>(2));
            var testb = new TestClassB();
            testb.Data = 1234;
            var buffer = ser.ObjectToBuffer(testb);
            var value = ser.BufferToObject(buffer) as TestClassB;

            Assert.AreEqual(testb.Data , value.Data);
        }


        [TestMethod()]
        public void Test3()
        {
            var ser = new Serializer(new NumberType<int>(1), new ClassType<TestClassB>(2) , new ClassArrayType<TestClassB>(3));
            var testb = new TestClassB();
            testb.Data = 1234;
            var buffer = ser.ObjectToBuffer(testb);
            var value = ser.BufferToObject(buffer) as TestClassB;

            Assert.AreEqual(testb.Data, value.Data);
        }

    }

    
}

