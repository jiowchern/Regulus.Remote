using System;
using System.Linq;
using System.CodeDom;
using NUnit.Framework;
using Regulus.Game;


namespace Regulus.Serialization.Tests
{
    public delegate void TestDelegate();
    public class SerializerTests
    {
        

        [NUnit.Framework.Test()]
        public void TypeIdentifier1Test()
        {
            

            var ti = new TypeIdentifier(typeof(TestDelegate) , null);
            
            Assert.AreEqual(0, ti.Describers.Count());
        }
        [NUnit.Framework.Test()]
        public void TypeIdentifier2Test()
        {
            var type = Type.GetType("System.Void*");            
            var ti = new TypeIdentifier(type, null);

            Assert.AreEqual(0, ti.Describers.Count());

        }
        [NUnit.Framework.Test()]
        public void NegativeIntNumberTest()
        {            
            var finder = new DescribersFinder(typeof(int) , typeof(uint));
            var provider = new DescriberProvider(finder.KeyDescriber, finder);
            var ser = new Serializer(provider);
            var buf = ser.ObjectToBuffer((int) -1);
            var val = (int) ser.BufferToObject(buf);

            Assert.AreEqual(-1, val);
        }

        [NUnit.Framework.Test()]
        public void NegativeLongNumberTest()
        {            
            var finder = new DescribersFinder(typeof(long) , typeof(uint));
            var provider = new DescriberProvider(finder.KeyDescriber , finder);
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

            
            var finder = new DescribersFinder(typeof(ulong));
            var provider = new DescriberProvider(finder.KeyDescriber , finder);


            
            var ser = new Serializer(provider);

            var buffer = ser.ObjectToBuffer(1UL);

            var value = ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(1UL, value);
        }


        [NUnit.Framework.Test()]
        public void ClassTest()
        {            
            var finder = new DescribersFinder(typeof(int) , typeof(TestClassB));
            var provider = new DescriberProvider(finder.KeyDescriber , finder);
            
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
            
            var finder = new DescribersFinder(typeof(int), typeof(TestClassB), typeof(TestClassB[]));
            var provider = new DescriberProvider(finder.KeyDescriber, finder);

            
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

            var finder = new DescribersFinder(typeof(byte) , typeof(short) , typeof(int) , typeof(long) , typeof(TEST1));
            var provider = new DescriberProvider(finder.KeyDescriber , finder);


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
            
            var finder = new DescribersFinder(typeof(int) , typeof(int[]));
            var provider = new DescriberProvider(finder.KeyDescriber , finder);

            
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
            
            var finder = new DescribersFinder(typeof(float) , typeof(float[]));
            var provider = new DescriberProvider(finder);


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
            var finder = new DescribersFinder(typeof(float));
            var provider = new DescriberProvider(finder);
            
            var ser = new Serializer(provider);

            var buffer = ser.ObjectToBuffer(123.43f);
            var value = (float) ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(123.43f, value);
        }


        [NUnit.Framework.Test()]
        public void ByteArrayStructTest()
        {            
            var finder = new DescribersFinder(typeof(byte[]));
            var provider = new DescriberProvider(finder);

            
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
            var finder = new DescribersFinder(typeof(char[]));
            var provider = new DescriberProvider(finder);
            
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
            
            var finder = new DescribersFinder(typeof(char[]));
            var provider = new DescriberProvider(finder);
            
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
            var finder = new DescribersFinder(typeof(Guid) ,typeof(Guid[]));
            var provider = new DescriberProvider(finder);
            
            var ser = new Serializer(provider);

            var id = Guid.NewGuid();
            var buffer = ser.ObjectToBuffer(id);
            var value = (Guid) ser.BufferToObject(buffer);

            NUnit.Framework.Assert.AreEqual(id, value);
        }


        [NUnit.Framework.Test()]
        public void ClassArrayHaveNullTest()
        {
            
            var finder = new DescribersFinder(typeof(TestClassC) , typeof(TestClassC[]));
            var provider = new DescriberProvider(finder);



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
            var finder = new DescribersFinder(typeof(TestClassC) , typeof(TestClassC[]));
            var provider = new DescriberProvider(finder);

            var ser = new Serializer(provider);

            var buffer = ser.ObjectToBuffer(null);
            var value = ser.BufferToObject(buffer) as TestClassC[];


            NUnit.Framework.Assert.AreEqual(null, value);

        }


        [NUnit.Framework.Test()]
        public void ByteArray1Test()
        {
            
            var finder = new DescribersFinder(typeof(byte) , typeof(byte[]));
            var provider = new DescriberProvider(finder);


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
            
            var finder = new DescribersFinder(typeof(byte[]) , typeof(int));
            var provider = new DescriberProvider(finder);


            
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
            
            var finder = new DescribersFinder(typeof(string) , typeof(char) , typeof(char[]));
            var provider = new DescriberProvider(finder);


            
            var ser = new Serializer(provider);


            var str = "fliwjfo3f3fnmsdlgmnlgrkmbr'nhmlredhgnedra'lngh";
            var buffer = ser.ObjectToBuffer(str);
            var value = ser.BufferToObject(buffer) as string;

            NUnit.Framework.Assert.AreEqual(str, value);
        }


        [NUnit.Framework.Test()]
        public void CharArrayTest()
        {
            
            var finder = new DescribersFinder(typeof(char) , typeof(char[]) );
            var provider = new DescriberProvider(finder);

            
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

            var ser = new Serializer(new DescriberBuilder(typeof(Regulus.CustomType.Vector2), typeof(float)).Describers);
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

            var ser = new Serializer(new DescriberBuilder(types).Describers);

            var intZeroBuffer = ser.ObjectToBuffer("123");

            var intZero = ser.BufferToObject(intZeroBuffer);


            Assert.AreEqual("123", intZero);
        }

        [NUnit.Framework.Test()]
        public void TestSerializerResponsePackage()
        {
            var types = new[] { typeof(Regulus.Remote.ResponsePackage) , typeof(Remote.ServerToClientOpCode) , typeof(byte), typeof(byte[]) };

            var ser = new Serializer(new DescriberBuilder(types).Describers);
            var pkg = new Regulus.Remote.ResponsePackage();
            pkg.Code = Remote.ServerToClientOpCode.SetProperty;
            pkg.Data = new byte[1] { 255 };
            var buffer = ser.ObjectToBuffer(pkg);

            var dPkg = ser.BufferToObject(buffer) as Remote.ResponsePackage;


            Assert.AreEqual(Remote.ServerToClientOpCode.SetProperty, dPkg.Code);
        }


    }

}

    

