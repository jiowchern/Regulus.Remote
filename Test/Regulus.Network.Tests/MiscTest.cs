using System;
using System.Collections.Generic;
using System.Net;

using NSubstitute;
using NUnit.Framework;
using Regulus.Network.RUDP;

namespace Regulus.Network.Tests
{

    public class MiscTest
	{


	    [Test]
        public void TestObjectPool()
	{
            
	    var pool = new ObjectPool<byte[] , ByteArrayShell>(new ByteArrayFactory() );
	    ByteArrayShell data1 = pool.Spawn();

            
        Assert.AreEqual(10, data1.Length);

	    _Set(data1);

	    
        _Set2(data1);
	    data1 = null;
        System.GC.Collect();
	    System.GC.WaitForFullGCComplete();


        ByteArrayShell data2 = pool.Spawn();
	    Assert.AreEqual(8, data2[0]);
	    

        ByteArrayShell data3 = pool.Spawn();
	    Assert.AreEqual(0, data3[0]);

        }

	    private static void _Set(ByteArrayShell data)
	    {
	        data[0] = 9;
	        data[1] = 8;
	        data[2] = 7;
	        data[3] = 6;
	        data[4] = 5;
	        data[5] = 4;
	        data[6] = 3;
	        data[7] = 2;
	        data[8] = 1;
	        data[9] = 0;
	        

	    }

	    private static void _Set2(ByteArrayShell data)
	    {
	        data[0] = 8;
        }

	    public class ByteArrayShell : IRecycleable<byte[]>
    {
        private byte[] _Instance;
                
        public byte this[int i]
        {
            get { return this._Instance[i]; }
            set { this._Instance[i] = value; }
        }

        public int Length { get { return _Instance.Length; } }


        void IRecycleable<byte[]>.Reset(byte[] instance)
        {
            _Instance = instance;
        }
    }

	    [Test]
        public void TestDataPackageSize()
	    {	        
            Assert.IsTrue(Config.Default.PackageSize - SocketMessage.GetHeadSize() > 0);	        
	    }





        [Test]
        public void TestBufferDispenser1()
	    {

	        var count = SocketMessage.GetPayloadSize() * 4;
            var message = new TestMessage(count);

	        
            var dispenser = new BufferDispenser(new IPEndPoint(IPAddress.Any, 0), SocketPackagePool.Instance);
	        var packages = dispenser.PackingTransmission(message.Buffer ,0,0);


            Assert.AreEqual( 4 , packages.Length );
	        byte index = 0;
	        int readcount = 0;
	        for (uint i = 0; i < packages.Length; i++)
	        {
	            var package = packages[i];
	            Assert.AreEqual(i, package.GetSeq());
                
                var data = new List<byte>();
	            package.ReadPayload(data);

                for (int j = 0; j < data.Count; j++)
	            {	                
	                Assert.AreEqual(index, data[j]);
	                index++;
	                readcount++;
	            }                
	        }

	        Assert.AreEqual(readcount, count);
        }

	    [Test]
        public void TestBufferDispenser2()
	    {
	        var count = SocketMessage.GetPayloadSize() * 4+1;
            var message = new TestMessage(count);


	        var dispenser = new BufferDispenser(new IPEndPoint(IPAddress.Any, 0), SocketPackagePool.Instance);
	        var packages = dispenser.PackingTransmission(message.Buffer, 0, 0);


	        Assert.AreEqual(5, packages.Length);
	        byte index = 0;
	        int readcount = 0;
	        for (uint i = 0; i < packages.Length; i++)
	        {
	            var package = packages[i];
	            Assert.AreEqual(i, package.GetSeq());

	            var data = new List<byte>();
	            package.ReadPayload(data);

	            for (int j = 0; j < data.Count; j++)
	            {
	                Assert.AreEqual(index, data[j]);
	                index++;
	                readcount++;

	            }
	        }

	        Assert.AreEqual(readcount, count);
        }

	    [Test]
        public void TestPackageRectifierOutOfOrder()
	    {
            var package1 = new SocketMessage(Config.Default.PackageSize);
	        package1.SetSeq(0);            
	        package1.WritePayload(new byte[] {1}, 0, 1);

            var package2 = new SocketMessage(Config.Default.PackageSize);
	        package2.SetSeq(1);
	        package2.WritePayload(new byte[] { 5 }, 0, 1);            

	        var package3 = new SocketMessage(Config.Default.PackageSize);
	        package3.SetSeq(2);
	        package3.WritePayload(new byte[] { 9 }, 0, 1);            


            var receiver = new PackageRectifier();
	        receiver.PushPackage(package3);
	        
	        var stream1 = receiver.PopPackage();
            Assert.AreEqual(null , stream1);

	        receiver.PushPackage(package2);

	        
	        var stream2 = receiver.PopPackage();
            Assert.AreEqual(null, stream2);


	        receiver.PushPackage(package1);
            
            Assert.AreEqual((byte)1, receiver.PopPackage().ReadPayload(0));
	        Assert.AreEqual((byte)5, receiver.PopPackage().ReadPayload(0));
	        Assert.AreEqual((byte)9, receiver.PopPackage().ReadPayload(0));

        }

	    [Test]
        public void TestPackageRectifierRepeat()
	    {
	        var package1 = new SocketMessage(Config.Default.PackageSize);
	        package1.SetSeq(0);
	        package1.WritePayload(new byte[] { 1 }, 0, 1);

	        var package2 = new SocketMessage(Config.Default.PackageSize);
	        package2.SetSeq(1);
	        package2.WritePayload(new byte[] { 5 }, 0, 1);

	        var package3 = new SocketMessage(Config.Default.PackageSize);
	        package3.SetSeq(2);
	        package3.WritePayload(new byte[] { 9 }, 0, 1);

            var package4 = new SocketMessage(Config.Default.PackageSize);
	        package4.SetSeq(3);
	        package4.WritePayload(new byte[] { 10 }, 0, 1);

	        var package5 = new SocketMessage(Config.Default.PackageSize);
	        package5.SetSeq(4);
	        package5.WritePayload(new byte[] { 11 }, 0, 1);


            var receiver = new PackageRectifier();
	        receiver.PushPackage(package3);
            
	        
            var stream1 = receiver.PopPackage();
	        Assert.AreEqual(null, stream1);

	        receiver.PushPackage(package2);
	        var stream2 = receiver.PopPackage();
	        Assert.AreEqual(null, stream2);

	        receiver.PushPackage(package1);
	        
            
            Assert.AreEqual((byte)1, receiver.PopPackage().ReadPayload(0));
	        Assert.AreEqual((byte)5, receiver.PopPackage().ReadPayload(0));
	        Assert.AreEqual((byte)9, receiver.PopPackage().ReadPayload(0));


            receiver.PushPackage(package5);
	        var stream4 = receiver.PopPackage();
	        Assert.AreEqual(null, stream4);

	        receiver.PushPackage(package2);
	        var stream5 = receiver.PopPackage();
	        Assert.AreEqual(null, stream5);

            receiver.PushPackage(package4);
	        	                    
	        Assert.AreEqual((byte)10, receiver.PopPackage().ReadPayload(0));
	        Assert.AreEqual((byte)11, receiver.PopPackage().ReadPayload(0));

        }

	    [Test]
        public void TestAck1()
	    {
	        var package1 = new SocketMessage(Config.Default.PackageSize);
	        package1.SetSeq(1);
            var package2 = new SocketMessage(Config.Default.PackageSize);
	        package2.SetSeq(2);

            var ackWaiter = new CongestionRecorder(3);

            
	        ackWaiter.PushWait(package1, (long)(Timestamp.OneSecondTicks*0.1));
	        ackWaiter.PushWait(package2, Timestamp.OneSecondTicks);
            ackWaiter.Reply(package1.GetSeq(), Timestamp.OneSecondTicks, 1);
	        var packages = ackWaiter.PopLost(Timestamp.OneSecondTicks * 100, Timestamp.OneSecondTicks * 100);

            Assert.AreEqual(2u , packages[0].GetSeq());


	    }

	    [Test]
        public void TestAck2()
	    {
	        var package1 = new SocketMessage(Config.Default.PackageSize);
	        package1.SetSeq(0);
	        var package2 = new SocketMessage(Config.Default.PackageSize);
	        package2.SetSeq(1);
	        var package3 = new SocketMessage(Config.Default.PackageSize);
	        package3.SetSeq(2);

            var ackWaiter = new CongestionRecorder(3);
	        var rectifier = new PackageRectifier();

	        ackWaiter.PushWait(package1 , 1);
	        ackWaiter.PushWait(package2 , 2);
	        ackWaiter.PushWait(package3, 3);

	        rectifier.PushPackage(package3);
            rectifier.PushPackage(package1);
	        

            ackWaiter.ReplyBefore((ushort)(rectifier.Serial - 1) ,1,1);
	        ackWaiter.ReplyAfter((ushort)(rectifier.Serial - 1) , rectifier.SerialBitFields,1,1);

	        Assert.AreEqual(1 ,ackWaiter.Count);

	        var outs = ackWaiter.PopLost(Timestamp.OneSecondTicks * 60, Timestamp.OneSecondTicks * 60 );

	        Assert.AreEqual(1, outs[0].GetSeq());
            



        }





    }

    public class TestSpawner : ISocketPackageSpawner
    {
        public SocketMessage Spawn()
        {
            return new SocketMessage(50);
        }
    }

    public class ByteArrayFactory : IObjectFactory<byte[]>
    {
        byte[] IObjectFactory<byte[]>.Spawn()
        {
            return new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
        }
    }

   

   

    public class TestMessage
    {
        public TestMessage(int byte_size)
        {
            Buffer = new byte[byte_size];
            for (int i = 0; i < byte_size; i++)
            {
                Buffer[i] = (byte)(i % 256);
            }
        }

        public readonly byte[] Buffer;
    }
}
