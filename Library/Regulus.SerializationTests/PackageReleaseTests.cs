
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Remoting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Regulus.Serialization;

namespace Regulus.Remoting.Tests
{
    [TestClass()]
    public class PackageReleaseTests
    {
        [TestMethod()]
        public void ToBufferTest1()
        {
            var id = Guid.NewGuid();
            var package1 = new TestPackageData();

            var ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(Guid) , typeof(TestPackageData)));
            package1.Id = id;

            var buffer = package1.ToBuffer(ser);

            var package2 = buffer.ToPackageData<TestPackageData>(ser);

            Assert.AreEqual(id , package2.Id);
        }

        [TestMethod()]
        public void ToBufferTest2()
        {
            
            var p1 = 0;
            var p2 = "234";
            var p3 = Guid.NewGuid();
            var package1 = new TestPackageBuffer();
            var ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int),typeof(string),typeof(char[]),typeof(byte) , typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestPackageBuffer)));


            package1.Datas = new [] { Regulus.TypeHelper.Serializer(p1), Regulus.TypeHelper.Serializer(p2), Regulus.TypeHelper.Serializer(p3) };

            var buffer = package1.ToBuffer(ser);

            var package2 = buffer.ToPackageData<TestPackageBuffer>(ser);

            
            Assert.AreEqual(p1, Regulus.TypeHelper.Deserialize<int>(package2.Datas[0]));
            Assert.AreEqual(p2, Regulus.TypeHelper.Deserialize<string>(package2.Datas[1]));
            Assert.AreEqual(p3, Regulus.TypeHelper.Deserialize<Guid>(package2.Datas[2]));
        }

        [TestMethod()]
        public void ToBufferTest3()
        {

            
            
            var package1 = new TestPackageBuffer();

            var ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestPackageBuffer)));

            package1.Datas = new byte[0][] ;

            var buffer = package1.ToBuffer(ser);

            var package2 = buffer.ToPackageData<TestPackageBuffer>(ser);


            Assert.AreEqual(0, package2.Datas.Length);
            
        }
    }
    [Serializable]
    
    public class TestPackageData : TPackageData<TestPackageData>
    {
    
        public Guid Id;
    }
    [Serializable]
    
    public class TestPackageBuffer : TPackageData<TestPackageBuffer>
    {

        public TestPackageBuffer()
        {
            Datas = new byte[0][];
        }
        
        public byte[][] Datas;
    }
}