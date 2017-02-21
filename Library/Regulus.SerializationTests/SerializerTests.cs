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
using NSubstitute;
namespace Regulus.Serialization.Tests
{
    [TestClass()]
    public class SerializerTests
    {
        

        

        [TestMethod()]
        public void SerializeTest3()
        {
            ITypeProvider intProvider = new StructTypeProvider<int>(1);
            ITypeProvider aProvider = new ClassTypeProvider<TestClassA>(2);
            ITypeProvider bProvider = new ClassTypeProvider<TestClassB>(3);
            var serializer = new Serializer(new[] {intProvider, aProvider, bProvider});

            var a = new TestClassA();
            a.B = new TestClassB();
            a.B.Data = 99;
            var buffer = serializer.Serialize(a);
            var val = serializer.Deserialize<TestClassA>(buffer);
            Assert.AreEqual(99, val.B.Data);
        }

        [TestMethod()]
        public void SerializeTest4()
        {
            ITypeProvider intProvider = new StructTypeProvider<int>(1);
            ITypeProvider aProvider = new ClassTypeProvider<TestClassC>(2);
            ITypeProvider bProvider = new ClassTypeProvider<TestClassB>(3);
            var serializer = new Serializer(new[] { intProvider, aProvider, bProvider });

            var c = new TestClassC();
            c.B1 = new TestClassB();
            c.B1.Data = 12345;
            c.B2 = new TestClassB();
            c.B2.Data = 54231;
            var buffer = serializer.Serialize(c);
            var val = serializer.Deserialize<TestClassC>(buffer);
            Assert.AreEqual(12345, val.B1.Data);
            Assert.AreEqual(54231, val.B2.Data);
        }

        [TestMethod()]
        public void SerializeTest5()
        {
            
            var serializer = new Serializer(new[]
            {
                new StructTypeProvider<int>(1),
                new EnumTypeProvider<TEST1>(2),
                
                new ClassTypeProvider<TestClassC>(3),
                new ClassTypeProvider<TestClassB>(4),
                new ClassTypeProvider<TestClassA>(5),
                new ClassTypeProvider<TestClassD>(6),

                new ClassArrayTypeProvider<TestClassC>(7),
                new ClassArrayTypeProvider<TestClassB>(8),
                new ClassArrayTypeProvider<TestClassA>(9),
                new ClassArrayTypeProvider<TestClassD>(10),
                new EnumArrayTypeProvider<TEST1>(11),
                new StructArrayTypeProvider<int>(12) as ITypeProvider,
            });

            var c1 = new TestClassC();
            c1.B2 = new TestClassB();
            var d1 = new TestClassD();
            d1.B = new TestClassB();
            d1.B.Data = 14711;
            d1.Cs = new TestClassC[] { c1, c1 };
            d1.T1 = TEST1.C;
            var ds = new[] {d1 , new TestClassD(), new TestClassD()};
            var buffer = serializer.Serialize(ds);
            var val = serializer.Deserialize<TestClassD[]>(buffer);
            Assert.AreEqual(TEST1.C, val[0].T1 );
            
        }

        [TestMethod()]
        public void SerializeTest6()
        {
            var serializer = new Serializer(new ITypeProvider[]
            {                
                new EnumTypeProvider<TEST1>(1) ,
                new EnumArrayTypeProvider<TEST1>(2),                                
            });
            var buffer = serializer.Serialize(new[] {TEST1.B, TEST1.A,});
            var t2 = serializer.Deserialize<TEST1[]>(buffer);
            Assert.AreEqual(TEST1.B, t2[0]);
            Assert.AreEqual(TEST1.A, t2[1]);
        }




        [TestMethod()]
        public void SerializeStructTypeProviderFloatTest()
        {
            ITypeProvider proivder = new StructTypeProvider<float>(1);

            var serializer = new Serializer(new[] {proivder});

            var buffer = serializer.Serialize(1.234f);
            var val = serializer.Deserialize<float>(buffer);
            Assert.AreEqual(1.234f, val);
        }

        [TestMethod()]
        public void SerializeStructTypeProviderInt64Test()
        {
            ITypeProvider proivder = new StructTypeProvider<Int64>(1);

            var serializer = new Serializer(new[] {proivder});

            Int64 data = 54;

            var buffer = serializer.Serialize(data);
            var val = serializer.Deserialize<Int64>(buffer);
            Assert.AreEqual(54, val);
        }


        [TestMethod()]
        public void SerializeStructTypeProviderInt64ArrayTest()
        {
            
            ITypeProvider proivder2 = new StructArrayTypeProvider<Int64>(2);

            var serializer = new Serializer(new[] {proivder2});

            Int64[] data = new Int64[] {64, 32};

            var buffer = serializer.Serialize(data);
            var val = serializer.Deserialize<Int64[]>(buffer);

            Assert.AreEqual(64, val[0]);
            Assert.AreEqual(32, val[1]);
        }





        

        [TestMethod()]
        public void DeserializeTest3()
        {
            var provider = new StructTypeProvider<int>(1);
            var serializer = new Serializer(new[] {provider});

            var buffer = new byte[]
            {
                1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, // head InstanceAmount  valueAmount
                // id parent parent_field 
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                // id begin end
                1, 0, 0, 0, 36, 0, 0, 0, 40, 0, 0, 0,
                100, 0, 0, 0 //data

            };
            var val = serializer.Deserialize<int>(buffer);
            Assert.AreEqual(100, val);
        }
        


        [TestMethod()]
        public void DeserializeTest4()
        {
            ITypeProvider provider1 = new StructTypeProvider<int>(1);
            ITypeProvider provider2 = new ClassTypeProvider<TestClassB>(2);
            ITypeProvider provider3 = new ClassArrayTypeProvider<TestClassB>(3);

            var serializer = new Serializer(new[] {provider1, provider2, provider3});

            var b1 = new TestClassB();
            b1.Data = 11;
            
            var b2 = new TestClassB();
            b2.Data = 22;

            var b3 = new TestClassB();
            b3.Data = 33;

            var bs = new[] {b1, null, b3};
            var buffer = serializer.Serialize(bs);

            var val = serializer.Deserialize<TestClassB[]>(buffer);
            Assert.AreEqual(11, val[0].Data);
            Assert.AreEqual(null, val[1]);
            Assert.AreEqual(33, val[2].Data);
        }

       
        [TestMethod()]
        public void DeserializeTest5()
        {
            var provider = new EnumTypeProvider<TEST1>(1);
            var serializer = new Serializer(new[] { provider });

            var buffer = new byte[]
            {
                1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, // head InstanceAmount  valueAmount
                // id parent parent_field 
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                // id begin end
                1, 0, 0, 0, 36, 0, 0, 0, 40, 0, 0, 0,
                1, 0, 0, 0 //data

            };
            var val = serializer.Deserialize<TEST1>(buffer);
            Assert.AreEqual(TEST1.B, val);
        }
    }
}
