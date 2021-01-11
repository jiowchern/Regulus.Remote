
using Regulus.Serialization;
using System;

namespace Regulus.Remote.Tests
{

    public class PackageReleaseTests
    {
        [Xunit.Fact]
        public void ToBufferTest1()
        {
            Guid id = Guid.NewGuid();
            TestPackageData package1 = new TestPackageData();

            Serializer ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(Guid), typeof(TestPackageData)).Describers);
            package1.Id = id;

            byte[] buffer = package1.ToBuffer(ser);

            TestPackageData package2 = buffer.ToPackageData<TestPackageData>(ser);

            Xunit.Assert.Equal(id, package2.Id);
        }

        [Xunit.Fact]
        public void ToBufferTest2()
        {

            int p1 = 0;
            string p2 = "234";
            Guid p3 = Guid.NewGuid();
            TestPackageBuffer package1 = new TestPackageBuffer();
            Serializer ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestPackageBuffer)).Describers);


            package1.Datas = new[] { ser.ObjectToBuffer(p1), ser.ObjectToBuffer(p2), ser.ObjectToBuffer(p3) };

            byte[] buffer = package1.ToBuffer(ser);

            TestPackageBuffer package2 = buffer.ToPackageData<TestPackageBuffer>(ser);


            Xunit.Assert.Equal(p1, ser.BufferToObject(package2.Datas[0]));
            Xunit.Assert.Equal(p2, ser.BufferToObject(package2.Datas[1]));
            Xunit.Assert.Equal(p3, ser.BufferToObject(package2.Datas[2]));
        }


        [Xunit.Fact]
        public void ToPackageRequestTest()
        {

            DescriberBuilder builder = new Regulus.Serialization.DescriberBuilder(
                            typeof(System.Int32),
                            typeof(System.Char),
                            typeof(System.Char[]),
                            typeof(System.String),
                            typeof(System.Boolean),
                            typeof(Regulus.Remote.RequestPackage),
                            typeof(System.Byte[]),
                            typeof(System.Byte),
                            typeof(Regulus.Remote.ClientToServerOpCode),
                            typeof(Regulus.Remote.ResponsePackage),
                            typeof(Regulus.Remote.ServerToClientOpCode),
                            typeof(System.Guid),
                            typeof(Regulus.Remote.PackageInvokeEvent),
                            typeof(System.Byte[][]),
                            typeof(Regulus.Remote.PackageErrorMethod),
                            typeof(Regulus.Remote.PackageReturnValue),
                            typeof(Regulus.Remote.PackageLoadSoulCompile),
                            typeof(Regulus.Remote.PackageLoadSoul),
                            typeof(Regulus.Remote.PackageUnloadSoul),
                            typeof(Regulus.Remote.PackageCallMethod),
                            typeof(Regulus.Remote.PackageRelease));
            Serializer ser = new Regulus.Serialization.Serializer(builder.Describers);
            RequestPackage response = new RequestPackage();
            response.Code = ClientToServerOpCode.Ping;
            response.Data = new byte[] { 0, 1, 2, 3, 4, 5 };

            byte[] bufferResponse = ser.ObjectToBuffer(response);
            RequestPackage result = ser.BufferToObject(bufferResponse) as RequestPackage;
            Xunit.Assert.Equal(ClientToServerOpCode.Ping, result.Code);
            Xunit.Assert.Equal(3, result.Data[3]);
        }






        [Xunit.Fact]
        public void ToBufferTest3()
        {



            TestPackageBuffer package1 = new TestPackageBuffer();

            Serializer ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestPackageBuffer)).Describers);

            package1.Datas = new byte[0][];

            byte[] buffer = package1.ToBuffer(ser);

            TestPackageBuffer package2 = buffer.ToPackageData<TestPackageBuffer>(ser);


            Xunit.Assert.Equal(0, package2.Datas.Length);

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