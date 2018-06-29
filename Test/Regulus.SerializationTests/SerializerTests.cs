using System;
using NUnit.Framework;


namespace Regulus.Serialization.Tests
{

    public class SerializerTests
    {

        [NUnit.Framework.Test()]
        public void NegativeIntNumberTest()
        {
            var provider = new DescriberProvider(new NumberDescriber<int>(), new NumberDescriber<uint>());
            var ser = new Serializer(provider);
            var buf = ser.ObjectToBuffer((int) -1);
            var val = (int) ser.BufferToObject(buf);

            Assert.AreEqual(-1, val);
        }

        [NUnit.Framework.Test()]
        public void NegativeLongNumberTest()
        {
            var provider = new DescriberProvider(new NumberDescriber<long>(), new NumberDescriber<uint>());
            var ser = new Serializer(provider);
            var buf = ser.ObjectToBuffer((long) -1);
            var val = (long) ser.BufferToObject(buf);

            Assert.AreEqual(-1, val);
        }




        [NUnit.Framework.Test()]
        public void EncodeZigZag()
        {
            var e64_1 = ZigZag.Encode((long) 1);
            var e64_2 = ZigZag.Encode(-1L);

            var e32_1 = ZigZag.Encode(1);
            var e32_2 = ZigZag.Encode(-1);


            NUnit.Framework.Assert.AreEqual((uint) 2, e32_1);
            NUnit.Framework.Assert.AreEqual((uint) 1, e32_2);

            NUnit.Framework.Assert.AreEqual((ulong) 2, e64_1);
            NUnit.Framework.Assert.AreEqual((ulong) 1, e64_2);


        }

        [NUnit.Framework.Test()]
        public void DecodeZigZag()
        {
            var e64_1 = ZigZag.Decode((ulong) 2);
            var e64_2 = ZigZag.Decode((ulong) 1);

            var e32_1 = ZigZag.Decode(2);
            var e32_2 = ZigZag.Decode(1);


            NUnit.Framework.Assert.AreEqual(1, e32_1);
            NUnit.Framework.Assert.AreEqual(-1, e32_2);

            NUnit.Framework.Assert.AreEqual(1, e64_1);
            NUnit.Framework.Assert.AreEqual(-1, e64_2);


        }

        [NUnit.Framework.Test()]
        public void ZigZagTest()
        {
            var value = ZigZag.Encode(874534L);
            var result = ZigZag.Decode(value);
            NUnit.Framework.Assert.AreEqual(874534L, result);
        }


        [NUnit.Framework.Test()]
        public void VarintToBufferTest()
        {

            var count = Varint.GetByteCount(150);
            var buffer = new byte[count];
            var index = Varint.NumberToBuffer(buffer, 0, 150);


            NUnit.Framework.Assert.AreEqual(0x96, buffer[0]);
            NUnit.Framework.Assert.AreEqual(0x1, buffer[1]);
            NUnit.Framework.Assert.AreEqual(2, index);

        }


        [NUnit.Framework.Test()]
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
            NUnit.Framework.Assert.AreEqual(150UL, number);
            NUnit.Framework.Assert.AreEqual(2, index);
        }


        [NUnit.Framework.Test()]
        public void VarintTest()
        {
            var count = Varint.GetByteCount(12671L);
            var buffer = new byte[count];
            Varint.NumberToBuffer(buffer, 0, 12671L);

            ulong value;
            Varint.BufferToNumber(buffer, 0, out value);

            NUnit.Framework.Assert.AreEqual(12671UL, value);
        }

        [NUnit.Framework.Test()]
        public void UlongTest()
        {
            var provider = new DescriberProvider(new NumberDescriber(typeof(ulong)));
            var ser = new Serializer(provider);

            var buffer = ser.ObjectToBuffer(1UL);

            var value = ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(1UL, value);
        }


        [NUnit.Framework.Test()]
        public void ClassTest()
        {
            var provider = new DescriberProvider(new NumberDescriber(typeof(int)),
                new ClassDescriber(typeof(TestClassB)));
            var ser = new Serializer(provider);
            var testb = new TestClassB();
            testb.Data = 1234;
            var buffer = ser.ObjectToBuffer(testb);
            var value = ser.BufferToObject(buffer) as TestClassB;

            NUnit.Framework.Assert.AreEqual(testb.Data, value.Data);
        }


        [NUnit.Framework.Test()]
        public void ClassArrayTest()
        {
            var provider = new DescriberProvider(new NumberDescriber(typeof(int)),
                new ClassDescriber(typeof(TestClassB)),
                new ArrayDescriber(typeof(TestClassB[])));
            var ser = new Serializer(provider);
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

            NUnit.Framework.Assert.AreEqual(null, value[0]);
            NUnit.Framework.Assert.AreEqual(1, value[1].Data);
            NUnit.Framework.Assert.AreEqual(null, value[2]);
            NUnit.Framework.Assert.AreEqual(3, value[3].Data);
            NUnit.Framework.Assert.AreEqual(null, value[4]);
        }


        [NUnit.Framework.Test()]
        public void NumberTest()
        {
            var provider = new DescriberProvider(new NumberDescriber(typeof(byte)),
                new NumberDescriber<short>(),
                new NumberDescriber<int>(),
                new NumberDescriber<long>(),
                new EnumDescriber<TEST1>());
            var ser = new Serializer(provider);

            var byteBuffer = ser.ObjectToBuffer((byte) 128);
            var byteValue = (byte) ser.BufferToObject(byteBuffer);

            var shortBuffer = ser.ObjectToBuffer((short) 16387);
            var shortValue = (short) ser.BufferToObject(shortBuffer);

            var intBuffer = ser.ObjectToBuffer((int) 65535);
            var intValue = (int) ser.BufferToObject(intBuffer);

            var longBuffer = ser.ObjectToBuffer((long) 65535000);
            var longValue = (long) ser.BufferToObject(longBuffer);

            var enumBuffer = ser.ObjectToBuffer(TEST1.C);
            var enumValue = (TEST1) ser.BufferToObject(enumBuffer);

            NUnit.Framework.Assert.AreEqual((byte) 128, byteValue);
            NUnit.Framework.Assert.AreEqual((short) 16387, shortValue);
            NUnit.Framework.Assert.AreEqual((int) 65535, intValue);
            NUnit.Framework.Assert.AreEqual((long) 65535000, longValue);
            NUnit.Framework.Assert.AreEqual(TEST1.C, enumValue);
        }


        [NUnit.Framework.Test()]
        public void IntArrayTest4()
        {
            var provider = new DescriberProvider(new NumberDescriber<int>(), new ArrayDescriber<int>());
            var ser = new Serializer(provider);

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
            var value = (int[]) ser.BufferToObject(buffer);


            NUnit.Framework.Assert.AreEqual(46, value[1]);
            NUnit.Framework.Assert.AreEqual(323, value[7]);
        }

        [NUnit.Framework.Test()]
        public void NumberFloatTest()
        {
            var provider = new DescriberProvider(new BlittableDescriber(typeof(float)),
                new ArrayDescriber<float>());
            var ser = new Serializer(provider);

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
            var value = (float[]) ser.BufferToObject(buffer);


            NUnit.Framework.Assert.AreEqual(46f, value[1]);
            NUnit.Framework.Assert.AreEqual(323f, value[7]);
        }


        [NUnit.Framework.Test()]
        public void StructFloatTest()
        {
            var provider = new DescriberProvider(new BlittableDescriber<float>());
            var ser = new Serializer(provider);

            var buffer = ser.ObjectToBuffer(123.43f);
            var value = (float) ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(123.43f, value);
        }


        [NUnit.Framework.Test()]
        public void ByteArrayStructTest()
        {
            var provider = new DescriberProvider(new BufferDescriber<byte[]>());
            var ser = new Serializer(provider);

            var buffer = ser.ObjectToBuffer(new byte[] {1, 2, 3, 4, 5, 6});
            var value = (byte[]) ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(1, value[0]);
            NUnit.Framework.Assert.AreEqual(2, value[1]);
            NUnit.Framework.Assert.AreEqual(3, value[2]);
            NUnit.Framework.Assert.AreEqual(4, value[3]);

        }

        [NUnit.Framework.Test()]
        public void CharArrayStructTest()
        {
            var provider = new DescriberProvider(new BufferDescriber<char[]>());
            var ser = new Serializer(provider);

            var buffer = ser.ObjectToBuffer(new char[] {'1', '2', 'a', 'b', 'c', 't'});
            var value = (char[]) ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual('1', value[0]);
            NUnit.Framework.Assert.AreEqual('2', value[1]);
            NUnit.Framework.Assert.AreEqual('a', value[2]);
            NUnit.Framework.Assert.AreEqual('b', value[3]);
            NUnit.Framework.Assert.AreEqual('c', value[4]);
            NUnit.Framework.Assert.AreEqual('t', value[5]);

        }

        [NUnit.Framework.Test()]
        public void StringCharArrayStructTest()
        {
            var provider = new DescriberProvider(new BufferDescriber<char[]>());
            var ser = new Serializer(provider);
            var str = "asdfgh";
            var buffer = ser.ObjectToBuffer(str.ToCharArray());
            var value = (char[]) ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual('a', value[0]);
            NUnit.Framework.Assert.AreEqual('s', value[1]);
            NUnit.Framework.Assert.AreEqual('d', value[2]);
            NUnit.Framework.Assert.AreEqual('f', value[3]);

        }





        [NUnit.Framework.Test()]
        public void GuidTest()
        {
            var provider = new DescriberProvider(new BlittableDescriber<Guid>(), new ArrayDescriber<Guid>());
            var ser = new Serializer(provider);

            var id = Guid.NewGuid();
            var buffer = ser.ObjectToBuffer(id);
            var value = (Guid) ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(id, value);
        }


        [NUnit.Framework.Test()]
        public void ClassArrayHaveNullTest()
        {
            var provider = new DescriberProvider(new ClassDescriber(typeof(TestClassC)),
                new ArrayDescriber<TestClassC>());
            var ser = new Serializer(provider);

            var cs = new TestClassC[]
            {
                null,
                new TestClassC(),
                null
            };


            var buffer = ser.ObjectToBuffer(cs);
            var value = ser.BufferToObject(buffer) as TestClassC[];


            NUnit.Framework.Assert.AreEqual(cs[0], value[0]);
            NUnit.Framework.Assert.AreNotEqual(null, value[1]);
            NUnit.Framework.Assert.AreEqual(cs[2], value[2]);
        }


        [NUnit.Framework.Test()]
        public void ClassNullTest()
        {
            var provider = new DescriberProvider(new ClassDescriber(typeof(TestClassC)),
                new ArrayDescriber<TestClassC>());
            var ser = new Serializer(provider);




            var buffer = ser.ObjectToBuffer(null);
            var value = ser.BufferToObject(buffer) as TestClassC[];


            NUnit.Framework.Assert.AreEqual(null, value);

        }


        [NUnit.Framework.Test()]
        public void ByteArray1Test()
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
            var provider = new DescriberProvider(new BlittableDescriber(typeof(byte)), new ArrayDescriber<byte>());
            var ser = new Serializer(provider);

            var buffer = ser.ObjectToBuffer(bytes);
            var result = ser.BufferToObject(buffer) as byte[];

            NUnit.Framework.Assert.AreEqual(bytes[3], result[3]);
            NUnit.Framework.Assert.AreEqual(bytes[1], result[1]);
            NUnit.Framework.Assert.AreEqual(bytes[8], result[8]);
            NUnit.Framework.Assert.AreEqual(bytes[9], result[9]);
        }


        [NUnit.Framework.Test()]
        public void ByteArray2Test()
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
            var provider = new DescriberProvider(new ByteArrayDescriber(), new NumberDescriber(typeof(int)));
            var ser = new Serializer(provider);

            var buffer = ser.ObjectToBuffer(bytes);
            var result = ser.BufferToObject(buffer) as byte[];

            NUnit.Framework.Assert.AreEqual(bytes[3], result[3]);
            NUnit.Framework.Assert.AreEqual(bytes[1], result[1]);
            NUnit.Framework.Assert.AreEqual(bytes[8], result[8]);
            NUnit.Framework.Assert.AreEqual(bytes[9], result[9]);
        }

        [NUnit.Framework.Test()]
        public void StringTest()
        {
            var provider = new DescriberProvider(new StringDescriber(), new NumberDescriber(typeof(char)),
                new ArrayDescriber(typeof(char[])));
            var ser = new Serializer(provider);


            var str = "fliwjfo3f3fnmsdlgmnlgrkmbr'nhmlredhgnedra'lngh";
            var buffer = ser.ObjectToBuffer(str);
            var value = ser.BufferToObject(buffer) as string;

            NUnit.Framework.Assert.AreEqual(str, value);
        }


        [NUnit.Framework.Test()]
        public void CharArrayTest()
        {
            var provider = new DescriberProvider(new NumberDescriber(typeof(char)), new ArrayDescriber(typeof(char[])));
            var ser = new Serializer(provider);


            var str = new char[] {'d', 'a'};
            var buffer = ser.ObjectToBuffer(str);
            var value = ser.BufferToObject(buffer) as char[];

            NUnit.Framework.Assert.AreEqual(str[0], value[0]);
            NUnit.Framework.Assert.AreEqual(str[1], value[1]);
        }


        [NUnit.Framework.Test()]
        public void TestSerializerVector2()
        {

            var ser = new Serializer(new DescriberBuilder(typeof(Regulus.CustomType.Vector2), typeof(float)));
            var v = new Regulus.CustomType.Vector2(99, 22);

            var array = ser.ObjectToBuffer(v);
            var v2 = (Regulus.CustomType.Vector2) ser.BufferToObject(array);

            NUnit.Framework.Assert.AreEqual(99, v2.X);
            NUnit.Framework.Assert.AreEqual(22, v2.Y);
        }


        [NUnit.Framework.Test()]
        public void TestSerializer1()
        {
            var types = new[] {typeof(int), typeof(int[]), typeof(float), typeof(string), typeof(char), typeof(char[])};

            var ser = new Serializer(new DescriberBuilder(types));

            var intZeroBuffer = ser.ObjectToBuffer("123");

            var intZero = ser.BufferToObject(intZeroBuffer);


            Assert.AreEqual("123", intZero);
        }

        
    }

}

    

