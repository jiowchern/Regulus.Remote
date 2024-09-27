using NUnit.Framework.Constraints;
using Regulus.Memorys;
using Regulus.Serialization;
using System;
using System.Linq;


namespace Regulus.Network.Tests
{
    public class PackageTest
    {
        struct TestStruct
        {
            public int A;            
        }
        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task Test1()
        {
            var serializer = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestStruct)).Describers, Regulus.Memorys.PoolProvider.DriectShared);

            var sendStream = new Stream();
            var readStream = new Regulus.Network.ReverseStream(sendStream);
            

            var sender = new Regulus.Network.PackageSender(sendStream, Regulus.Memorys.PoolProvider.DriectShared);
            var testStruct = new TestStruct();
            testStruct.A = -1;
            
            var testStructBuffer = serializer.ObjectToBuffer(testStruct);

            await sender.Send(testStructBuffer);


            var reader = new Regulus.Network.PackageReader(readStream, Regulus.Memorys.PoolProvider.DriectShared);

            
            
            var buffers = await reader.Read();
            var buffer = buffers.Single();


            var readStruct = (TestStruct)serializer.BufferToObject(buffer)  ;
            NUnit.Framework.Assert.AreEqual(testStruct.A, readStruct.A);
            

        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task Test2()
        {
            var serializer = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestStruct)).Describers, Regulus.Memorys.PoolProvider.DriectShared);

            var sendStream = new Stream();
            var readStream = new Regulus.Network.ReverseStream(sendStream);


            var sender = new Regulus.Network.PackageSender(sendStream, Regulus.Memorys.PoolProvider.DriectShared);
            var testStructBuffer1 = serializer.ObjectToBuffer(null);
            await sender.Send(testStructBuffer1);

            var testStructBuffer2 = serializer.ObjectToBuffer(1);
            await sender.Send(testStructBuffer2);

            var testStructBuffer3 = serializer.ObjectToBuffer(2);
            await sender.Send(testStructBuffer3);
            var testStruct = new TestStruct();
            testStruct.A = -1;
            var testStructBuffer4 = serializer.ObjectToBuffer(testStruct);
            await sender.Send(testStructBuffer4);




            var reader = new Regulus.Network.PackageReader(readStream, Regulus.Memorys.PoolProvider.DriectShared);



            var buffers = await reader.Read();
            {
                var buffer = buffers.ElementAt(0);
                var readStruct = serializer.BufferToObject(buffer);
                NUnit.Framework.Assert.AreEqual(null, readStruct);
            }
            for (var i = 1; i < 3; i++)
            {
                var buffer = buffers.ElementAt(i);
                var readStruct = (int)serializer.BufferToObject(buffer);
                NUnit.Framework.Assert.AreEqual(i, readStruct);
            }
            {
                var buffer = buffers.ElementAt(3);
                var readStruct = (TestStruct)serializer.BufferToObject(buffer);
                NUnit.Framework.Assert.AreEqual(-1, readStruct.A);
            }
            





        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task Test3()
        {
            var serializer = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestStruct)).Describers, Regulus.Memorys.PoolProvider.DriectShared);

            var sendStream = new Stream();
            var readStream = new Regulus.Network.ReverseStream(sendStream);


            var sender = new Regulus.Network.PackageSender(sendStream, Regulus.Memorys.PoolProvider.DriectShared);
            
            await sender.Send(Regulus.Memorys.PoolProvider.DriectShared.Alloc(0));

            var testStructBuffer2 = serializer.ObjectToBuffer(null);
            await sender.Send(testStructBuffer2);

            var testStructBuffer3 = serializer.ObjectToBuffer(null);
            await sender.Send(testStructBuffer2);

            var reader = new Regulus.Network.PackageReader(readStream, Regulus.Memorys.PoolProvider.DriectShared);

            var buffers = await reader.Read();
            
            for (var i = 0; i < 2; i++)
            {
                var buffer = buffers.ElementAt(i);
                var readStruct = serializer.BufferToObject(buffer);
                NUnit.Framework.Assert.AreEqual(null, readStruct);
            }
        }
    }
}
