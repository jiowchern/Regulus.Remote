

using Regulus.Remoting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Regulus.Serialization;

namespace Regulus.Remoting.Tests
{
    
    public class PackageReleaseTests
    {
        [NUnit.Framework.Test()]
        public void ToBufferTest1()
        {
            var id = Guid.NewGuid();
            var package1 = new TestPackageData();

            var ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(Guid) , typeof(TestPackageData)).Describers);
            package1.Id = id;

            var buffer = package1.ToBuffer(ser);

            var package2 = buffer.ToPackageData<TestPackageData>(ser);

            NUnit.Framework.Assert.AreEqual(id , package2.Id);
        }

        [NUnit.Framework.Test()]
        public void ToBufferTest2()
        {
            
            var p1 = 0;
            var p2 = "234";
            var p3 = Guid.NewGuid();
            var package1 = new TestPackageBuffer();
            var ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int),typeof(string),typeof(char[]),typeof(byte) , typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestPackageBuffer)).Describers);


            package1.Datas = new [] { ser.ObjectToBuffer(p1), ser.ObjectToBuffer(p2), ser.ObjectToBuffer(p3) };

            var buffer = package1.ToBuffer(ser);

            var package2 = buffer.ToPackageData<TestPackageBuffer>(ser);

            
            NUnit.Framework.Assert.AreEqual(p1, ser.BufferToObject(package2.Datas[0]));
            NUnit.Framework.Assert.AreEqual(p2, ser.BufferToObject(package2.Datas[1]));
            NUnit.Framework.Assert.AreEqual(p3, ser.BufferToObject(package2.Datas[2]));
        }


        [NUnit.Framework.Test()]
        public void ToPackageRequestTest()
        {

            var builder = new Regulus.Serialization.DescriberBuilder(
                            typeof(System.Int32),
                            typeof(System.Char),
                            typeof(System.Char[]),
                            typeof(System.String),
                            typeof(System.Boolean),
                            typeof(Regulus.Remoting.RequestPackage),
                            typeof(System.Byte[]),
                            typeof(System.Byte),
                            typeof(Regulus.Remoting.ClientToServerOpCode),
                            typeof(Regulus.Remoting.ResponsePackage),
                            typeof(Regulus.Remoting.ServerToClientOpCode),
                            typeof(Regulus.Remoting.PackageUpdateProperty),
                            typeof(System.Guid),
                            typeof(Regulus.Remoting.PackageInvokeEvent),
                            typeof(System.Byte[][]),
                            typeof(Regulus.Remoting.PackageErrorMethod),
                            typeof(Regulus.Remoting.PackageReturnValue),
                            typeof(Regulus.Remoting.PackageLoadSoulCompile),
                            typeof(Regulus.Remoting.PackageLoadSoul),
                            typeof(Regulus.Remoting.PackageUnloadSoul),
                            typeof(Regulus.Remoting.PackageCallMethod),
                            typeof(Regulus.Remoting.PackageRelease));
            var ser = new Regulus.Serialization.Serializer(builder.Describers);
            var response = new RequestPackage();
            response.Code = ClientToServerOpCode.Ping;
            response.Data = new byte[] {0,1,2,3,4,5};

            var bufferResponse = ser.ObjectToBuffer(response);
            var result = ser.BufferToObject(bufferResponse) as RequestPackage; 
            NUnit.Framework.Assert.AreEqual(ClientToServerOpCode.Ping , result.Code);
            NUnit.Framework.Assert.AreEqual(3, result.Data[3]);
        }


        [NUnit.Framework.Test()]
        public void ToPackageResponseTest()
        {

            var builder = new Regulus.Serialization.DescriberBuilder(
                            typeof(System.Int32),
                            typeof(System.Char),
                            typeof(System.Char[]),
                            typeof(System.String),
                            typeof(System.Boolean),
                            typeof(Regulus.Remoting.RequestPackage),
                            typeof(System.Byte[]),
                            typeof(System.Byte),
                            typeof(Regulus.Remoting.ClientToServerOpCode),
                            typeof(Regulus.Remoting.ResponsePackage),
                            typeof(Regulus.Remoting.ServerToClientOpCode),
                            typeof(Regulus.Remoting.PackageUpdateProperty),
                            typeof(System.Guid),
                            typeof(Regulus.Remoting.PackageInvokeEvent),
                            typeof(System.Byte[][]),
                            typeof(Regulus.Remoting.PackageErrorMethod),
                            typeof(Regulus.Remoting.PackageReturnValue),
                            typeof(Regulus.Remoting.PackageLoadSoulCompile),
                            typeof(Regulus.Remoting.PackageLoadSoul),
                            typeof(Regulus.Remoting.PackageUnloadSoul),
                            typeof(Regulus.Remoting.PackageCallMethod),
                            typeof(Regulus.Remoting.PackageRelease));
            var ser = new Regulus.Serialization.Serializer(builder.Describers);
            var response = new ResponsePackage();
            response.Code = ServerToClientOpCode.UpdateProperty;
            response.Data = new byte[] { 0, 1, 2, 3, 4, 5 };

            var bufferResponse = ser.ObjectToBuffer(response);
            var result = ser.BufferToObject(bufferResponse) as ResponsePackage;
            NUnit.Framework.Assert.AreEqual(ServerToClientOpCode.UpdateProperty, result.Code);
            NUnit.Framework.Assert.AreEqual(3, result.Data[3]);
        }

        [NUnit.Framework.Test()]
        public void ToPackageUpdateTest()
        {

            var builder = new Regulus.Serialization.DescriberBuilder(
                            typeof(System.Char),
                            typeof(System.Char[]),
                            typeof(System.String),
                            typeof(System.Boolean),
                            typeof(Regulus.Remoting.RequestPackage),
                            typeof(System.Byte[]),
                            typeof(System.Byte),
                            typeof(Regulus.Remoting.ClientToServerOpCode),
                            typeof(Regulus.Remoting.ResponsePackage),
                            typeof(Regulus.Remoting.ServerToClientOpCode),
                            typeof(Regulus.Remoting.PackageUpdateProperty),
                            typeof(System.Guid),
                            typeof(System.Int32),
                            typeof(Regulus.Remoting.PackageInvokeEvent),
                            typeof(System.Byte[][]),
                            typeof(Regulus.Remoting.PackageErrorMethod),
                            typeof(Regulus.Remoting.PackageReturnValue),
                            typeof(Regulus.Remoting.PackageLoadSoulCompile),
                            typeof(Regulus.Remoting.PackageLoadSoul),
                            typeof(Regulus.Remoting.PackageUnloadSoul),
                            typeof(Regulus.Remoting.PackageCallMethod),
                            typeof(Regulus.Remoting.PackageRelease));
            var ser = new Regulus.Serialization.Serializer(builder.Describers);
            var update = new PackageUpdateProperty();
            update.Property = 1;
            update.EntityId = new Guid("3ecae85d-79e0-4cc9-a34f-60f31883d26c");
            update.Args = ser.ObjectToBuffer("kdw");

            var buf = ser.ObjectToBuffer(update);
            var result = ser.BufferToObject(buf) as PackageUpdateProperty;

            var name = ser.BufferToObject(result.Args) as string;

            NUnit.Framework.Assert.AreEqual(result.EntityId , update.EntityId);
            NUnit.Framework.Assert.AreEqual("kdw", name);


        }


        [NUnit.Framework.Test()]
        public void ToBufferTest3()
        {

            
            
            var package1 = new TestPackageBuffer();

            var ser = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestPackageBuffer)).Describers);

            package1.Datas = new byte[0][] ;

            var buffer = package1.ToBuffer(ser);

            var package2 = buffer.ToPackageData<TestPackageBuffer>(ser);


            NUnit.Framework.Assert.AreEqual(0, package2.Datas.Length);
            
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