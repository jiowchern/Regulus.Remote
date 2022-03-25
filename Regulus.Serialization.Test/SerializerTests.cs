using NUnit.Framework;
using System;
using System.Linq;



namespace Regulus.Serialization.Tests
{
    public delegate void TestDelegate();
    public class SerializerTests
    {


        [NUnit.Framework.Test]
        public void TypeIdentifier1Test()
        {


            TypeIdentifier ti = new TypeIdentifier(typeof(TestDelegate), null);

            Assert.AreEqual(0, ti.Describers.Count());
        }
        [NUnit.Framework.Test]
        public void TypeIdentifier2Test()
        {
            Type type = Type.GetType("System.Void*");
            TypeIdentifier ti = new TypeIdentifier(type, null);

            Assert.AreEqual(0, ti.Describers.Count());

        }
        [NUnit.Framework.Test]
        public void NegativeIntNumberTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(int), typeof(uint));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);
            Serializer ser = new Serializer(provider);
            byte[] buf = ser.ObjectToBuffer((int)-1);
            int val = (int)ser.BufferToObject(buf);

            Assert.AreEqual(-1, val);
        }

        [NUnit.Framework.Test]
        public void NegativeLongNumberTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(long), typeof(uint));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);
            Serializer ser = new Serializer(provider);
            byte[] buf = ser.ObjectToBuffer((long)-1);
            long val = (long)ser.BufferToObject(buf);

            Assert.AreEqual(-1, val);
        }




        [NUnit.Framework.Test]
        public void EncodeZigZag()
        {
            ulong e64_1 = ZigZag.Encode((long)1);
            ulong e64_2 = ZigZag.Encode(-1L);

            uint e32_1 = ZigZag.Encode(1);
            uint e32_2 = ZigZag.Encode(-1);


            NUnit.Framework.Assert.AreEqual((uint)2, e32_1);
            NUnit.Framework.Assert.AreEqual((uint)1, e32_2);

            NUnit.Framework.Assert.AreEqual((ulong)2, e64_1);
            NUnit.Framework.Assert.AreEqual((ulong)1, e64_2);


        }

        [NUnit.Framework.Test]
        public void DecodeZigZag()
        {
            long e64_1 = ZigZag.Decode((ulong)2);
            long e64_2 = ZigZag.Decode((ulong)1);

            int e32_1 = ZigZag.Decode(2);
            int e32_2 = ZigZag.Decode(1);


            NUnit.Framework.Assert.AreEqual(1, e32_1);
            NUnit.Framework.Assert.AreEqual(-1, e32_2);

            NUnit.Framework.Assert.AreEqual(1, e64_1);
            NUnit.Framework.Assert.AreEqual(-1, e64_2);


        }

        [NUnit.Framework.Test]
        public void ZigZagTest()
        {
            ulong value = ZigZag.Encode(874534L);
            long result = ZigZag.Decode(value);
            NUnit.Framework.Assert.AreEqual(874534L, result);
        }


        [NUnit.Framework.Test]
        public void VarintToBufferTest()
        {

            int count = Varint.GetByteCount(150);
            byte[] buffer = new byte[count];
            int index = Varint.NumberToBuffer(buffer, 0, 150);


            NUnit.Framework.Assert.AreEqual(0x96, buffer[0]);
            NUnit.Framework.Assert.AreEqual(0x1, buffer[1]);
            NUnit.Framework.Assert.AreEqual(2, index);

        }


        [NUnit.Framework.Test]
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
            NUnit.Framework.Assert.AreEqual(150UL, number);
            NUnit.Framework.Assert.AreEqual(2, index);
        }


        [NUnit.Framework.Test]
        public void VarintTest()
        {
            int count = Varint.GetByteCount(12671L);
            byte[] buffer = new byte[count];
            Varint.NumberToBuffer(buffer, 0, 12671L);

            ulong value;
            Varint.BufferToNumber(buffer, 0, out value);

            NUnit.Framework.Assert.AreEqual(12671UL, value);
        }

        [NUnit.Framework.Test]
        public void UlongTest()
        {


            DescribersFinder finder = new DescribersFinder(typeof(ulong));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);



            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(1UL);

            object value = ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(1UL, value);
        }


        [NUnit.Framework.Test]
        public void ClassTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(int), typeof(TestClassB));
            DescriberProvider provider = new DescriberProvider(finder.KeyDescriber, finder);

            Serializer ser = new Serializer(provider);
            TestClassB testb = new TestClassB();
            testb.Data = 1234;
            byte[] buffer = ser.ObjectToBuffer(testb);
            TestClassB value = ser.BufferToObject(buffer) as TestClassB;

            NUnit.Framework.Assert.AreEqual(testb.Data, value.Data);
        }

       

        [NUnit.Framework.Test]
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

            NUnit.Framework.Assert.AreEqual(null, value[0]);
            NUnit.Framework.Assert.AreEqual(1, value[1].Data);
            NUnit.Framework.Assert.AreEqual(null, value[2]);
            NUnit.Framework.Assert.AreEqual(3, value[3].Data);
            NUnit.Framework.Assert.AreEqual(null, value[4]);
        }


        [NUnit.Framework.Test]
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

            NUnit.Framework.Assert.AreEqual((byte)128, byteValue);
            NUnit.Framework.Assert.AreEqual((short)16387, shortValue);
            NUnit.Framework.Assert.AreEqual((int)65535, intValue);
            NUnit.Framework.Assert.AreEqual((long)65535000, longValue);
            NUnit.Framework.Assert.AreEqual(TEST1.C, enumValue);
        }


        [NUnit.Framework.Test]
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


            NUnit.Framework.Assert.AreEqual(46, value[1]);
            NUnit.Framework.Assert.AreEqual(323, value[7]);
        }

        [NUnit.Framework.Test]
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


            NUnit.Framework.Assert.AreEqual(46f, value[1]);
            NUnit.Framework.Assert.AreEqual(323f, value[7]);
        }


        [NUnit.Framework.Test]
        public void StructFloatTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(float));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(123.43f);
            float value = (float)ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(123.43f, value);
        }


        [NUnit.Framework.Test]
        public void ByteArrayStructTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(byte[]));
            DescriberProvider provider = new DescriberProvider(finder);


            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(new byte[] { 1, 2, 3, 4, 5, 6 });
            byte[] value = (byte[])ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(1, value[0]);
            NUnit.Framework.Assert.AreEqual(2, value[1]);
            NUnit.Framework.Assert.AreEqual(3, value[2]);
            NUnit.Framework.Assert.AreEqual(4, value[3]);

        }

        [NUnit.Framework.Test]
        public void CharArrayStructTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(char[]));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(new char[] { '1', '2', 'a', 'b', 'c', 't' });
            char[] value = (char[])ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual('1', value[0]);
            NUnit.Framework.Assert.AreEqual('2', value[1]);
            NUnit.Framework.Assert.AreEqual('a', value[2]);
            NUnit.Framework.Assert.AreEqual('b', value[3]);
            NUnit.Framework.Assert.AreEqual('c', value[4]);
            NUnit.Framework.Assert.AreEqual('t', value[5]);

        }

        [NUnit.Framework.Test]
        public void StringCharArrayStructTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(char[]));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);
            string str = "asdfgh";
            byte[] buffer = ser.ObjectToBuffer(str.ToCharArray());
            char[] value = (char[])ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual('a', value[0]);
            NUnit.Framework.Assert.AreEqual('s', value[1]);
            NUnit.Framework.Assert.AreEqual('d', value[2]);
            NUnit.Framework.Assert.AreEqual('f', value[3]);

        }





        [NUnit.Framework.Test]
        public void GuidTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(Guid), typeof(Guid[]));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);

            Guid id = Guid.NewGuid();
            byte[] buffer = ser.ObjectToBuffer(id);
            Guid value = (Guid)ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(id, value);
        }


        [NUnit.Framework.Test]
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


            NUnit.Framework.Assert.AreEqual(cs[0], value[0]);
            
            NUnit.Framework.Assert.AreNotEqual(null, value[1]);
            NUnit.Framework.Assert.AreEqual(cs[2], value[2]);
        }


        [NUnit.Framework.Test]
        public void ClassNullTest()
        {
            DescribersFinder finder = new DescribersFinder(typeof(TestClassC), typeof(TestClassC[]));
            DescriberProvider provider = new DescriberProvider(finder);

            Serializer ser = new Serializer(provider);

            byte[] buffer = ser.ObjectToBuffer(null);
            TestClassC[] value = ser.BufferToObject(buffer) as TestClassC[];


            NUnit.Framework.Assert.AreEqual(null, value);

        }


        [NUnit.Framework.Test]
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

            NUnit.Framework.Assert.AreEqual(bytes[3], result[3]);
            NUnit.Framework.Assert.AreEqual(bytes[1], result[1]);
            NUnit.Framework.Assert.AreEqual(bytes[8], result[8]);
            NUnit.Framework.Assert.AreEqual(bytes[9], result[9]);
        }


        [NUnit.Framework.Test]
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

            NUnit.Framework.Assert.AreEqual(bytes[3], result[3]);
            NUnit.Framework.Assert.AreEqual(bytes[1], result[1]);
            NUnit.Framework.Assert.AreEqual(bytes[8], result[8]);
            NUnit.Framework.Assert.AreEqual(bytes[9], result[9]);
        }

        [NUnit.Framework.Test]
        public void StringTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(string), typeof(char), typeof(char[]));
            DescriberProvider provider = new DescriberProvider(finder);



            Serializer ser = new Serializer(provider);


            string str = "fliwjfo3f3fnmsdlgmnlgrkmbr'nhmlredhgnedra'lngh";
            byte[] buffer = ser.ObjectToBuffer(str);
            string value = ser.BufferToObject(buffer) as string;

            NUnit.Framework.Assert.AreEqual(str, value);
        }


        [NUnit.Framework.Test]
        public void CharArrayTest()
        {

            DescribersFinder finder = new DescribersFinder(typeof(char), typeof(char[]));
            DescriberProvider provider = new DescriberProvider(finder);


            Serializer ser = new Serializer(provider);


            char[] str = new char[] { 'd', 'a' };
            byte[] buffer = ser.ObjectToBuffer(str);
            char[] value = ser.BufferToObject(buffer) as char[];

            NUnit.Framework.Assert.AreEqual(str[0], value[0]);
            NUnit.Framework.Assert.AreEqual(str[1], value[1]);
        }


        [NUnit.Framework.Test]
        public void TestSerializerVector2()
        {

            Serializer ser = new Serializer(new DescriberBuilder(typeof(Regulus.Utility.Vector2), typeof(float)).Describers);
            Utility.Vector2 v = new Regulus.Utility.Vector2(99, 22);

            byte[] array = ser.ObjectToBuffer(v);
            Utility.Vector2 v2 = (Regulus.Utility.Vector2)ser.BufferToObject(array);

            NUnit.Framework.Assert.AreEqual(99, v2.X);
            NUnit.Framework.Assert.AreEqual(22, v2.Y);
        }


        [NUnit.Framework.Test]
        public void TestSerializer1()
        {
            Type[] types = new[] { typeof(int), typeof(int[]), typeof(float), typeof(string), typeof(char), typeof(char[]) };

            Serializer ser = new Serializer(new DescriberBuilder(types).Describers);

            byte[] intZeroBuffer = ser.ObjectToBuffer("123");

            object intZero = ser.BufferToObject(intZeroBuffer);


            Assert.AreEqual("123", intZero);
        }

        [NUnit.Framework.Test]
        public void TestSerializerResponsePackage()
        {
            Type[] types = new[] { typeof(Regulus.Remote.Packages.ResponsePackage), typeof(Remote.ServerToClientOpCode), typeof(byte), typeof(byte[]) };

            Serializer ser = new Serializer(new DescriberBuilder(types).Describers);
            Regulus.Remote.Packages.ResponsePackage pkg = new Regulus.Remote.Packages.ResponsePackage();
            pkg.Code = Remote.ServerToClientOpCode.SetProperty;
            pkg.Data = new byte[1] { 255 };
            byte[] buffer = ser.ObjectToBuffer(pkg);

            Regulus.Remote.Packages.ResponsePackage dPkg = (Regulus.Remote.Packages.ResponsePackage)ser.BufferToObject(buffer)  ;


            Assert.AreEqual(Remote.ServerToClientOpCode.SetProperty, dPkg.Code);
        }

        

        
    }

}



