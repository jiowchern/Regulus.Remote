using System;
using System.CodeDom;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using NSubstitute.Exceptions;

namespace Regulus.Serialization.Tests
{
    [TestClass()]
    public class SerializerTests
    {





        [TestMethod()]
        public void EncodeZigZag()
        {
            var e64_1 = ZigZag.Encode((long)1);
            var e64_2 = ZigZag.Encode(-1L);

            var e32_1 = ZigZag.Encode(1);
            var e32_2 = ZigZag.Encode(-1);


            Assert.AreEqual((uint)2, e32_1);
            Assert.AreEqual((uint)1, e32_2);

            Assert.AreEqual((ulong)2, e64_1);
            Assert.AreEqual((ulong)1, e64_2);


        }

        [TestMethod()]
        public void DecodeZigZag()
        {
            var e64_1 = ZigZag.Decode((ulong)2);
            var e64_2 = ZigZag.Decode((ulong)1);

            var e32_1 = ZigZag.Decode(2);
            var e32_2 = ZigZag.Decode(1);


            Assert.AreEqual(1, e32_1);
            Assert.AreEqual(-1, e32_2);

            Assert.AreEqual(1, e64_1);
            Assert.AreEqual(-1, e64_2);


        }

        [TestMethod()]
        public void ZigZagTest()
        {
            var value = ZigZag.Encode(874534L);
            var result = ZigZag.Decode(value);
            Assert.AreEqual(874534L, result);
        }


        [TestMethod()]
        public void VarintToBufferTest()
        {

            var count = Varint.GetByteCount(150);
            var buffer = new byte[count];
            var index = Varint.NumberToBuffer(buffer, 0, 150);


            Assert.AreEqual(0x96, buffer[0]);
            Assert.AreEqual(0x1, buffer[1]);
            Assert.AreEqual(2, index);

        }


        [TestMethod()]
        public void VarintToNumberTest()
        {
            ulong number;
            var index = Varint.BufferToNumber(
                new byte[]
                {
                    0x96,
                    0x1
                },
                0,
                out number);
            Assert.AreEqual(150UL, number);
            Assert.AreEqual(2, index);
        }


        [TestMethod()]
        public void VarintTest()
        {
            var count = Varint.GetByteCount(12671L);
            var buffer = new byte[count];
            Varint.NumberToBuffer(buffer, 0, 12671L);

            ulong value;
            Varint.BufferToNumber(buffer, 0, out value);

            Assert.AreEqual(12671UL, value);
        }

        [TestMethod()]
        public void UlongTest()
        {

            var ser = new Serializer(new NumberDescriber(1, typeof (ulong)));

            var buffer = ser.ObjectToBuffer(1UL);

            var value = ser.BufferToObject(buffer);

            Assert.AreEqual(1UL, value);
        }


        [TestMethod()]
        public void ClassTest()
        {
            var ser = new Serializer(new NumberDescriber(1, typeof (int)), new ClassDescriber(2, typeof (TestClassB)));
            var testb = new TestClassB();
            testb.Data = 1234;
            var buffer = ser.ObjectToBuffer(testb);
            var value = ser.BufferToObject(buffer) as TestClassB;

            Assert.AreEqual(testb.Data, value.Data);
        }


        [TestMethod()]
        public void ClassArrayTest()
        {
            var ser = new Serializer(
                new NumberDescriber(1, typeof (int)),
                new ClassDescriber(2, typeof (TestClassB)),
                new ArrayDescriber(3, typeof (TestClassB[])));
            var testbs = new[]
            {
                null,
                new TestClassB()
                {
                    Data = 1
                },
                null,
                new TestClassB()
                {
                    Data = 3
                },
                null,
            };

            var buffer = ser.ObjectToBuffer(testbs);
            var value = ser.BufferToObject(buffer) as TestClassB[];

            Assert.AreEqual(null, value[0]);
            Assert.AreEqual(1, value[1].Data);
            Assert.AreEqual(null, value[2]);
            Assert.AreEqual(3, value[3].Data);
            Assert.AreEqual(null, value[4]);
        }


        [TestMethod()]
        public void NumberTest()
        {
            var ser = new Serializer(
                new NumberDescriber(1, typeof (byte)),
                new NumberDescriber<short>(2),
                new NumberDescriber<int>(3),
                new NumberDescriber<long>(4),
                new EnumDescriber<TEST1>(5)
                );
            var byteBuffer = ser.ObjectToBuffer((byte)128);
            var byteValue = (byte)ser.BufferToObject(byteBuffer);

            var shortBuffer = ser.ObjectToBuffer((short)16387);
            var shortValue = (short)ser.BufferToObject(shortBuffer);

            var intBuffer = ser.ObjectToBuffer((int)65535);
            var intValue = (int)ser.BufferToObject(intBuffer);

            var longBuffer = ser.ObjectToBuffer((long)65535000);
            var longValue = (long)ser.BufferToObject(longBuffer);

            var enumBuffer = ser.ObjectToBuffer(TEST1.C);
            var enumValue = (TEST1)ser.BufferToObject(enumBuffer);

            Assert.AreEqual((byte)128, byteValue);
            Assert.AreEqual((short)16387, shortValue);
            Assert.AreEqual((int)65535, intValue);
            Assert.AreEqual((long)65535000, longValue);
            Assert.AreEqual(TEST1.C, enumValue);
        }


        [TestMethod()]
        public void IntArrayTest4()
        {
            var ser = new Serializer(new NumberDescriber<int>(1), new ArrayDescriber<int>(2));

            var ints = new[]
            {
                4,
                46,
                6,
                8,
                8,
                4,
                32,
                323,
                78
            };
            var buffer = ser.ObjectToBuffer(ints);
            var value = (int[])ser.BufferToObject(buffer);


            Assert.AreEqual(46, value[1]);
            Assert.AreEqual(323, value[7]);
        }

        [TestMethod()]
        public void NumberFloatTest()
        {
            var ser = new Serializer(new NumberDescriber<float>(1), new ArrayDescriber<float>(2));

            var ints = new[]
            {
                4f,
                46f,
                6f,
                8f,
                8f,
                4f,
                32f,
                323f,
                78f
            };
            var buffer = ser.ObjectToBuffer(ints);
            var value = (float[])ser.BufferToObject(buffer);


            Assert.AreEqual(46f, value[1]);
            Assert.AreEqual(323f, value[7]);
        }


        [TestMethod()]
        public void StructFloatTest()
        {
            var ser = new Serializer(new StructDescriber<float>(1));

            var buffer = ser.ObjectToBuffer(123.43f);
            var value = (float)ser.BufferToObject(buffer);

            Assert.AreEqual(123.43f, value);
        }


        [TestMethod()]
        public void GuidTest()
        {
            var ser = new Serializer(new StructDescriber<Guid>(1), new ArrayDescriber<Guid>(2));

            var id = Guid.NewGuid();
            var buffer = ser.ObjectToBuffer(id);
            var value = (Guid)ser.BufferToObject(buffer);

            Assert.AreEqual(id, value);
        }


        [TestMethod()]
        public void ClassArrayHaveNullTest()
        {
            var ser = new Serializer(new ClassDescriber(1, typeof (TestClassC)), new ArrayDescriber<TestClassC>(2));

            var cs = new TestClassC[]
            {
                null,
                new TestClassC(),
                null
            };


            var buffer = ser.ObjectToBuffer(cs);
            var value = ser.BufferToObject<TestClassC[]>(buffer);


            Assert.AreEqual(cs[0], value[0]);
            Assert.AreNotEqual(null, value[1]);
            Assert.AreEqual(cs[2], value[2]);
        }


        [TestMethod()]
        public void ByteArrayTest()
        {
            var bytes = new byte[]
            {
                0x5,
                97,
                2,
                92,
                9,
                113,
                2,
                4,
                7,
                6,
                9,
                255,
                0
            };

            var ser = new Serializer(new StructDescriber(1, typeof (byte)), new ArrayDescriber<byte>(2));

            var buffer = ser.ObjectToBuffer(bytes);
            var result = ser.BufferToObject(buffer) as byte[];

            Assert.AreEqual(bytes[3], result[3]);
            Assert.AreEqual(bytes[1], result[1]);
            Assert.AreEqual(bytes[8], result[8]);
            Assert.AreEqual(bytes[9], result[9]);
        }

        [TestMethod()]
        public void StringTest()
        {

            var ser = new Serializer(new StringDescriber(1) , new NumberDescriber(2 , typeof(char)) , new ArrayDescriber(3 , typeof(char[])) );
            

            var str = "fliwjfo3f3fnmsdlgmnlgrkmbr'nhmlredhgnedra'lngh";
            var buffer = ser.ObjectToBuffer(str);
            var value = ser.BufferToObject(buffer) as string;

            Assert.AreEqual(str ,value );
        }


        [TestMethod()]
        public void CharArrayTest()
        {

            var ser = new Serializer(new NumberDescriber(2, typeof(char)), new ArrayDescriber(3, typeof(char[])));


            var str = new char[] {'d' ,'a'};
            var buffer = ser.ObjectToBuffer(str);
            var value = ser.BufferToObject(buffer) as char[];

            Assert.AreEqual(str[0], value[0]);
            Assert.AreEqual(str[1], value[1]);
        }


        [TestMethod]
        public void TestSerializerVector2()
        {

            var ser = new Serializer(new DescriberBuilder(typeof(Regulus.CustomType.Vector2) , typeof(float)));
            var v = new Regulus.CustomType.Vector2(99, 22);

            var array = ser.ObjectToBuffer(v);
            var v2 = (Regulus.CustomType.Vector2)ser.BufferToObject(array);

            Assert.AreEqual(99, v2.X);
            Assert.AreEqual(22, v2.Y);
        }


        [TestMethod]
        public void TestPolygonSerializ()
        {
            var ser = new Serializer(new DescriberBuilder(typeof(Regulus.CustomType.Vector2), typeof(float) , typeof(Regulus.CustomType.Polygon)));

            var polygon1 = new Regulus.CustomType.Polygon();
            polygon1.SetPoints(new[]{
                    new Regulus.CustomType.Vector2(0,0),
                    new Regulus.CustomType.Vector2(1,0),
                    new Regulus.CustomType.Vector2(1,1),
                    new Regulus.CustomType.Vector2(0,1)});

            var buffer = ser.ObjectToBuffer(polygon1);
            var polygon2 = (Regulus.CustomType.Polygon)ser.BufferToObject(buffer) ;


            Assert.AreEqual(polygon2.Points[0], polygon1.Points[0]);

        }
    }

    
}

    

