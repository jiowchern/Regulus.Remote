using Xunit;
using System;
using System.Linq;



namespace Regulus.Serialization.Tests
{
    public delegate void TestDelegate();
    public class SerializerTests
    {


        [Xunit.Fact]
        public void TypeIdentifier1Test()
        {


            TypeIdentifier ti = new TypeIdentifier(typeof(TestDelegate), null);

            Assert.Equal(0, ti.Describers.Count());
        }
        [Xunit.Fact]
        public void TypeIdentifier2Test()
        {
            Type type = Type.GetType("System.Void*");
            TypeIdentifier ti = new TypeIdentifier(type, null);

            Assert.Equal(0, ti.Describers.Count());

        }
        [Xunit.Fact]
        public void NegativeIntNumberTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(int), typeof(uint));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);
            Serializer ser = new Serializer(provider);
            byte[] buf = ser.ObjectToBuffer((int)-1);
            int val = (int)ser.BufferToObject(buf);

            Assert.Equal(-1, val);
        }

        [Xunit.Fact]
        public void NegativeLongNumberTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(long), typeof(uint));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);
            Serializer ser = new Serializer(provider);
            byte[] buf = ser.ObjectToBuffer((long)-1);
            long val = (long)ser.BufferToObject(buf);

            Assert.Equal(-1, val);
        }




        [Xunit.Fact]
        public void EncodeZigZag()
        {
            ulong e64_1 = ZigZag.Encode((long)1);
            ulong e64_2 = ZigZag.Encode(-1L);

            uint e32_1 = ZigZag.Encode(1);
            uint e32_2 = ZigZag.Encode(-1);


            Xunit.Assert.Equal((uint)2, e32_1);
            Xunit.Assert.Equal((uint)1, e32_2);

            Xunit.Assert.Equal((ulong)2, e64_1);
            Xunit.Assert.Equal((ulong)1, e64_2);


        }

        [Xunit.Fact]
        public void DecodeZigZag()
        {
            long e64_1 = ZigZag.Decode((ulong)2);
            long e64_2 = ZigZag.Decode((ulong)1);

            int e32_1 = ZigZag.Decode(2);
            int e32_2 = ZigZag.Decode(1);


            Xunit.Assert.Equal(1, e32_1);
            Xunit.Assert.Equal(-1, e32_2);

            Xunit.Assert.Equal(1, e64_1);
            Xunit.Assert.Equal(-1, e64_2);


        }

        [Xunit.Fact]
        public void ZigZagTest()
        {
            ulong value = ZigZag.Encode(874534L);
            long result = ZigZag.Decode(value);
            Xunit.Assert.Equal(874534L, result);
        }


        [Xunit.Fact]
        public void VarintToBufferTest()
        {

            int count = Varint.GetByteCount(150);
            byte[] buffer = new byte[count];
            int index = Varint.NumberToBuffer(buffer, 0, 150);


            Xunit.Assert.Equal(0x96, buffer[0]);
            Xunit.Assert.Equal(0x1, buffer[1]);
            Xunit.Assert.Equal(2, index);

        }


        [Xunit.Fact]
        public void VarintToNumberTest()
        {
            ulong number;
            int index = Varint.BufferToNumber(
                new byte[]
                {
                    0x96,
                    0x1
                },
                0,
                out number);
            Xunit.Assert.Equal(150UL, number);
            Xunit.Assert.Equal(2, index);
        }


        [Xunit.Fact]
        public void VarintTest()
        {
            int count = Varint.GetByteCount(12671L);
            byte[] buffer = new byte[count];
            Varint.NumberToBuffer(buffer, 0, 12671L);

            ulong value;
            Varint.BufferToNumber(buffer, 0, out value);

            Xunit.Assert.Equal(12671UL, value);
        }

        [Xunit.Fact]
        public void UlongTest()
        {


            DescribersFinder finder = new DescribersFinder(typeof(ulong));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);



            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(1UL);

            object value = ser.BufferToObject(buffer);

            Xunit.Assert.Equal(1UL, value);
        }


        [Xunit.Fact]
        public void ClassTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(int), typeof(TestClassB));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);

            Serializer ser = new Serializer(provider);
            TestClassB testb = new TestClassB();
            testb.Data = 1234;
            byte[] buffer = ser.ObjectToBuffer(testb);
            TestClassB value = ser.BufferToObject(buffer) as TestClassB;

            Xunit.Assert.Equal(testb.Data, value.Data);
        }


        [Xunit.Fact]
        public void ClassArrayTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(int), typeof(TestClassB), typeof(TestClassB[]));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);


            Serializer ser = new Serializer(provider);
            TestClassB[] testbs = new[]
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

            byte[] buffer = ser.ObjectToBuffer(testbs);
            TestClassB[] value = ser.BufferToObject(buffer) as TestClassB[];

            Xunit.Assert.Equal(null, value[0]);
            Xunit.Assert.Equal(1, value[1].Data);
            Xunit.Assert.Equal(null, value[2]);
            Xunit.Assert.Equal(3, value[3].Data);
            Xunit.Assert.Equal(null, value[4]);
        }


        [Xunit.Fact]
        public void NumberTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(byte), typeof(short), typeof(int), typeof(long), typeof(TEST1));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);


            Serializer ser = new Serializer(provider);

            byte[] byteBuffer = ser.ObjectToBuffer((byte)128);
            byte byteValue = (byte)ser.BufferToObject(byteBuffer);

            byte[] shortBuffer = ser.ObjectToBuffer((short)16387);
            short shortValue = (short)ser.BufferToObject(shortBuffer);

            byte[] intBuffer = ser.ObjectToBuffer((int)65535);
            int intValue = (int)ser.BufferToObject(intBuffer);

            byte[] longBuffer = ser.ObjectToBuffer((long)65535000);
            long longValue = (long)ser.BufferToObject(longBuffer);

            byte[] enumBuffer = ser.ObjectToBuffer(TEST1.C);
            TEST1 enumValue = (TEST1)ser.BufferToObject(enumBuffer);

            Xunit.Assert.Equal((byte)128, byteValue);
            Xunit.Assert.Equal((short)16387, shortValue);
            Xunit.Assert.Equal((int)65535, intValue);
            Xunit.Assert.Equal((long)65535000, longValue);
            Xunit.Assert.Equal(TEST1.C, enumValue);
        }


        [Xunit.Fact]
        public void IntArrayTest4()
        {

            DescribersFinder finder = new DescribersFinder(typeof(int), typeof(int[]));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);


            Serializer ser = new Serializer(provider);

            int[] ints = new[]
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
            byte[] buffer = ser.ObjectToBuffer(ints);
            int[] value = (int[])ser.BufferToObject(buffer);


            Xunit.Assert.Equal(46, value[1]);
            Xunit.Assert.Equal(323, value[7]);
        }

        [Xunit.Fact]
        public void NumberFloatTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(float), typeof(float[]));
            DescriberProvider provider = new DescriberProvider(finder);


            Serializer ser = new Serializer(provider);

            float[] ints = new[]
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
            byte[] buffer = ser.ObjectToBuffer(ints);
            float[] value = (float[])ser.BufferToObject(buffer);


            Xunit.Assert.Equal(46f, value[1]);
            Xunit.Assert.Equal(323f, value[7]);
        }


        [Xunit.Fact]
        public void StructFloatTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(float));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(123.43f);
            float value = (float)ser.BufferToObject(buffer);

            Xunit.Assert.Equal(123.43f, value);
        }


        [Xunit.Fact]
        public void ByteArrayStructTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(byte[]));
            DescriberProvider provider = new DescriberProvider(finder);


            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(new byte[] { 1, 2, 3, 4, 5, 6 });
            byte[] value = (byte[])ser.BufferToObject(buffer);

            Xunit.Assert.Equal(1, value[0]);
            Xunit.Assert.Equal(2, value[1]);
            Xunit.Assert.Equal(3, value[2]);
            Xunit.Assert.Equal(4, value[3]);

        }

        [Xunit.Fact]
        public void CharArrayStructTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(char[]));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(new char[] { '1', '2', 'a', 'b', 'c', 't' });
            char[] value = (char[])ser.BufferToObject(buffer);

            Xunit.Assert.Equal('1', value[0]);
            Xunit.Assert.Equal('2', value[1]);
            Xunit.Assert.Equal('a', value[2]);
            Xunit.Assert.Equal('b', value[3]);
            Xunit.Assert.Equal('c', value[4]);
            Xunit.Assert.Equal('t', value[5]);

        }

        [Xunit.Fact]
        public void StringCharArrayStructTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(char[]));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);
            string str = "asdfgh";
            byte[] buffer = ser.ObjectToBuffer(str.ToCharArray());
            char[] value = (char[])ser.BufferToObject(buffer);

            Xunit.Assert.Equal('a', value[0]);
            Xunit.Assert.Equal('s', value[1]);
            Xunit.Assert.Equal('d', value[2]);
            Xunit.Assert.Equal('f', value[3]);

        }





        [Xunit.Fact]
        public void GuidTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(Guid), typeof(Guid[]));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);

            Guid id = Guid.NewGuid();
            byte[] buffer = ser.ObjectToBuffer(id);
            Guid value = (Guid)ser.BufferToObject(buffer);

            Xunit.Assert.Equal(id, value);
        }


        [Xunit.Fact]
        public void ClassArrayHaveNullTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(TestClassC), typeof(TestClassC[]));
            DescriberProvider provider = new DescriberProvider(finder);



            Serializer ser = new Serializer(provider);

            TestClassC[] cs = new TestClassC[]
            {
                null,
                new TestClassC(),
                null
            };


            byte[] buffer = ser.ObjectToBuffer(cs);
            TestClassC[] value = ser.BufferToObject(buffer) as TestClassC[];


            Xunit.Assert.Equal(cs[0], value[0]);
            Xunit.Assert.NotEqual(null, value[1]);
            Xunit.Assert.Equal(cs[2], value[2]);
        }


        [Xunit.Fact]
        public void ClassNullTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(TestClassC), typeof(TestClassC[]));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(null);
            TestClassC[] value = ser.BufferToObject(buffer) as TestClassC[];


            Xunit.Assert.Equal(null, value);

        }


        [Xunit.Fact]
        public void ByteArray1Test()
        {

            DescribersFinder finder = new DescribersFinder(typeof(byte), typeof(byte[]));
            DescriberProvider provider = new DescriberProvider(finder);


            byte[] bytes = new byte[]
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

            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(bytes);
            byte[] result = ser.BufferToObject(buffer) as byte[];

            Xunit.Assert.Equal(bytes[3], result[3]);
            Xunit.Assert.Equal(bytes[1], result[1]);
            Xunit.Assert.Equal(bytes[8], result[8]);
            Xunit.Assert.Equal(bytes[9], result[9]);
        }


        [Xunit.Fact]
        public void ByteArray2Test()
        {
            byte[] bytes = new byte[]
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

            DescribersFinder finder = new DescribersFinder(typeof(byte[]), typeof(int));
            DescriberProvider provider = new DescriberProvider(finder);



            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(bytes);
            byte[] result = ser.BufferToObject(buffer) as byte[];

            Xunit.Assert.Equal(bytes[3], result[3]);
            Xunit.Assert.Equal(bytes[1], result[1]);
            Xunit.Assert.Equal(bytes[8], result[8]);
            Xunit.Assert.Equal(bytes[9], result[9]);
        }

        [Xunit.Fact]
        public void StringTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(string), typeof(char), typeof(char[]));
            DescriberProvider provider = new DescriberProvider(finder);



            Serializer ser = new Serializer(provider);


            string str = "fliwjfo3f3fnmsdlgmnlgrkmbr'nhmlredhgnedra'lngh";
            byte[] buffer = ser.ObjectToBuffer(str);
            string value = ser.BufferToObject(buffer) as string;

            Xunit.Assert.Equal(str, value);
        }


        [Xunit.Fact]
        public void CharArrayTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(char), typeof(char[]));
            DescriberProvider provider = new DescriberProvider(finder);


            Serializer ser = new Serializer(provider);


            char[] str = new char[] { 'd', 'a' };
            byte[] buffer = ser.ObjectToBuffer(str);
            char[] value = ser.BufferToObject(buffer) as char[];

            Xunit.Assert.Equal(str[0], value[0]);
            Xunit.Assert.Equal(str[1], value[1]);
        }


        [Xunit.Fact]
        public void TestSerializerVector2()
        {

            Serializer ser = new Serializer(new DescriberBuilder(typeof(Regulus.Utility.Vector2), typeof(float)).Describers);
            Utility.Vector2 v = new Regulus.Utility.Vector2(99, 22);

            byte[] array = ser.ObjectToBuffer(v);
            Utility.Vector2 v2 = (Regulus.Utility.Vector2)ser.BufferToObject(array);

            Xunit.Assert.Equal(99, v2.X);
            Xunit.Assert.Equal(22, v2.Y);
        }


        [Xunit.Fact]
        public void TestSerializer1()
        {
            Type[] types = new[] { typeof(int), typeof(int[]), typeof(float), typeof(string), typeof(char), typeof(char[]) };

            Serializer ser = new Serializer(new DescriberBuilder(types).Describers);

            byte[] intZeroBuffer = ser.ObjectToBuffer("123");

            object intZero = ser.BufferToObject(intZeroBuffer);


            Assert.Equal("123", intZero);
        }

        [Xunit.Fact]
        public void TestSerializerResponsePackage()
        {
            Type[] types = new[] { typeof(Regulus.Remote.ResponsePackage), typeof(Remote.ServerToClientOpCode), typeof(byte), typeof(byte[]) };

            Serializer ser = new Serializer(new DescriberBuilder(types).Describers);
            Remote.ResponsePackage pkg = new Regulus.Remote.ResponsePackage();
            pkg.Code = Remote.ServerToClientOpCode.SetProperty;
            pkg.Data = new byte[1] { 255 };
            byte[] buffer = ser.ObjectToBuffer(pkg);

            Remote.ResponsePackage dPkg = ser.BufferToObject(buffer) as Remote.ResponsePackage;


            Assert.Equal(Remote.ServerToClientOpCode.SetProperty, dPkg.Code);
        }


    }

}



