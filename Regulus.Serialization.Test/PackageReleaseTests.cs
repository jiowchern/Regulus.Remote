
using Regulus.Serialization;
using System;
using System.Linq;

namespace Regulus.Remote.Tests
{

    public class PackageReleaseTests
    {
        [NUnit.Framework.Test]
        public void ToBufferTest1()
        {
            Guid id = Guid.NewGuid();
            TestPackageData package1 = new TestPackageData();

            var ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(Guid), typeof(TestPackageData)).Describers);
            package1.Id = id;

            
            byte[] buffer = ser.ObjectToBuffer(package1).ToArray();

            
            TestPackageData package2 = ser.BufferToObject(buffer) as TestPackageData;

            NUnit.Framework.Assert.AreEqual(id, package2.Id);
        }

        [NUnit.Framework.Test]
        public void ToBufferTest2()
        {

            int p1 = 0;
            string p2 = "234";
            Guid p3 = Guid.NewGuid();
            TestPackageBuffer package1 = new TestPackageBuffer();
            var ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestPackageBuffer)).Describers);


            package1.Datas = new[] { ser.ObjectToBuffer(p1).ToArray(), ser.ObjectToBuffer(p2).ToArray(), ser.ObjectToBuffer(p3).ToArray() };

            //byte[] buffer = package1.ToBuffer(ser);
            byte[] buffer = ser.ObjectToBuffer(package1).ToArray();

            //TestPackageBuffer package2 = buffer.ToPackageData<TestPackageBuffer>(ser);
            TestPackageBuffer package2 = ser.BufferToObject(buffer) as TestPackageBuffer;


            NUnit.Framework.Assert.AreEqual(p1, ser.BufferToObject(package2.Datas[0]));
            NUnit.Framework.Assert.AreEqual(p2, ser.BufferToObject(package2.Datas[1]));
            NUnit.Framework.Assert.AreEqual(p3, ser.BufferToObject(package2.Datas[2]));
        }


        [NUnit.Framework.Test]
        public void ToPackageRequestTest()
        {

            DescriberBuilder builder = new Regulus.Serialization.DescriberBuilder(
                            typeof(System.Int32),
                            typeof(System.Char),
                            typeof(System.Char[]),
                            typeof(System.String),
                            typeof(System.Boolean),
                            typeof(Regulus.Remote.Packages.RequestPackage),
                            typeof(System.Byte[]),
                            typeof(System.Byte),
                            typeof(Regulus.Remote.ClientToServerOpCode),
                            typeof(Regulus.Remote.Packages.ResponsePackage),
                            typeof(Regulus.Remote.ServerToClientOpCode),
                            typeof(System.Guid),
                            typeof(Regulus.Remote.Packages.PackageInvokeEvent),
                            typeof(System.Byte[][]),
                            typeof(Regulus.Remote.Packages.PackageErrorMethod),
                            typeof(Regulus.Remote.Packages.PackageReturnValue),
                            typeof(Regulus.Remote.Packages.PackageLoadSoulCompile),
                            typeof(Regulus.Remote.Packages.PackageLoadSoul),
                            typeof(Regulus.Remote.Packages.PackageUnloadSoul),
                            typeof(Regulus.Remote.Packages.PackageCallMethod),
                            typeof(Regulus.Remote.Packages.PackageRelease));
            var ser = new Regulus.Serialization.Serializer(builder.Describers);
            Regulus.Remote.Packages.RequestPackage response = new Regulus.Remote.Packages.RequestPackage();
            response.Code = ClientToServerOpCode.Ping;
            response.Data = new byte[] { 0, 1, 2, 3, 4, 5 };

            byte[] bufferResponse = ser.ObjectToBuffer(response).ToArray();
            Regulus.Remote.Packages.RequestPackage result = (Regulus.Remote.Packages.RequestPackage)ser.BufferToObject(bufferResponse)  ;
            NUnit.Framework.Assert.AreEqual(ClientToServerOpCode.Ping, result.Code);
            NUnit.Framework.Assert.AreEqual(3, result.Data[3]);
        }






        [NUnit.Framework.Test]
        public void ToBufferTest3()
        {



            TestPackageBuffer package1 = new TestPackageBuffer();

            var ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestPackageBuffer)).Describers);

            package1.Datas = new byte[0][];

            //byte[] buffer = package1.ToBuffer(ser);
            byte[] buffer = ser.ObjectToBuffer(package1).ToArray();

            TestPackageBuffer package2 = ser.BufferToObject(buffer) as TestPackageBuffer;


            NUnit.Framework.Assert.AreEqual(0, package2.Datas.Length);

        }
    }
    [Serializable]

    public class TestPackageData 
    {

        public Guid Id;
    }
    [Serializable]

    public class TestPackageBuffer 
    {

        public TestPackageBuffer()
        {
            Datas = new byte[0][];
        }

        public byte[][] Datas;
    }



}