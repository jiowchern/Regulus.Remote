using NSubstitute.Extensions;
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
            var serializer = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestStruct)).Describers, Regulus.Memorys.PoolProvider.DirectShared);

            var sendStream = new Stream();
            var readStream = new Regulus.Network.ReverseStream(sendStream);
            
            
            var sender = new Regulus.Network.PackageSender(sendStream, Regulus.Memorys.PoolProvider.Shared);
            var testStruct = new TestStruct();
            testStruct.A = -1;
            
            var testStructBuffer = serializer.ObjectToBuffer(testStruct);

            sender.Push(testStructBuffer);


            var reader = new Regulus.Network.PackageReader(readStream, Regulus.Memorys.PoolProvider.Shared);

            
            
            var buffers = await reader.Read();
            var buffer = buffers.Single();


            var readStruct = (TestStruct)serializer.BufferToObject(buffer)  ;
            NUnit.Framework.Assert.AreEqual(testStruct.A, readStruct.A);
            

        }

        [NUnit.Framework.Test]
        public async System.Threading.Tasks.Task Test2()
        {
            var serializer = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestStruct)).Describers, Regulus.Memorys.PoolProvider.DirectShared);

            var sendStream = new Stream();
            var readStream = new Regulus.Network.ReverseStream(sendStream);

            
            var sender = new Regulus.Network.PackageSender(sendStream, Regulus.Memorys.PoolProvider.Shared);
            var testStructBuffer1 = serializer.ObjectToBuffer(null);
            sender.Push(testStructBuffer1);

            var testStructBuffer2 = serializer.ObjectToBuffer(1);
            sender.Push(testStructBuffer2);

            var testStructBuffer3 = serializer.ObjectToBuffer(2);
            sender.Push(testStructBuffer3);
            var testStruct = new TestStruct();
            testStruct.A = -1;
            var testStructBuffer4 = serializer.ObjectToBuffer(testStruct);
            sender.Push(testStructBuffer4);




            var reader = new Regulus.Network.PackageReader(readStream, Regulus.Memorys.PoolProvider.Shared);

            System.Collections.Generic.List<Regulus.Memorys.Buffer> buffers = new System.Collections.Generic.List<Memorys.Buffer>();

            while(buffers.Count < 4)
            {
                var bufs = await reader.Read();
                if (bufs.Count == 0)
                    break;
                buffers.AddRange(bufs);
            }
            
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
            var serializer = new Regulus.Serialization.Serializer(new DescriberBuilder(typeof(int), typeof(string), typeof(char[]), typeof(byte), typeof(byte[]), typeof(byte[][]), typeof(char), typeof(Guid), typeof(TestStruct)).Describers, Regulus.Memorys.PoolProvider.DirectShared);

            var sendStream = new Stream();
            var readStream = new Regulus.Network.ReverseStream(sendStream);

            
            var sender = new Regulus.Network.PackageSender(sendStream, Regulus.Memorys.PoolProvider.Shared);

            var buff = Regulus.Memorys.PoolProvider.DirectShared.Alloc(0);
            sender.Push(buff);

            var testStructBuffer2 = serializer.ObjectToBuffer(null);
            sender.Push(testStructBuffer2);

            var testStructBuffer3 = serializer.ObjectToBuffer(null);
            sender.Push(testStructBuffer3);

            var reader = new Regulus.Network.PackageReader(readStream, Regulus.Memorys.PoolProvider.Shared);

            var buffers = new System.Collections.Generic.List<Regulus.Memorys.Buffer>();
            while (buffers.Count < 2)
            {
                var bufs = await reader.Read();
                if (bufs.Count == 0)
                    break;

                buffers.AddRange(bufs);
            }
            
            for (var i = 0; i < 2; i++)
            {
                var buffer = buffers.ElementAt(i);
                var readStruct = serializer.BufferToObject(buffer);
                NUnit.Framework.Assert.AreEqual(null, readStruct);
            }

            
        }
    }
}
