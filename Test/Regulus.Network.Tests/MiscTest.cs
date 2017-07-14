using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Regulus.Network.RUDP;

namespace Regulus.Network.Tests
{
	[TestClass]
	public class MiscTest
	{


	[TestMethod]
	public void TestObjectPool()
	{
            
	    var pool = new ObjectPool<byte[] , ByteArrayShell>(new ByteArrayFactory() );
	    ByteArrayShell data = pool.Spawn();

            
        Assert.AreEqual(10, data.Length);

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

	    data = null;
        System.GC.Collect();
	    System.GC.WaitForFullGCComplete();


	    ByteArrayShell data2 = pool.Spawn();
	    Assert.AreEqual(9, data2[0]);

    }

    public class ByteArrayShell : Recycleable<byte[]>
    {
        
        public byte this[int i]
        {
            get { return this._Object[i]; }
            set { this._Object[i] = value; }
        }

        public int Length { get { return _Object.Length; } }


            
    }

    [TestMethod]
	    public void TestDataPackageSize()
	    {	        
            Assert.IsTrue(Config.PackageSize - SegmentPackage.GetHeadSize() > 0);	        
	    }

	    	    

	    

	    [TestMethod]
	    public void TestBufferDispenser()
	    {
	        
            var message = new TestMessage(155);	        

            var dispenser = new BufferDispenser( 50);
	        var packages = dispenser.Packing(message.Buffer ,PEER_OPERATION.TRANSMISSION);


            Assert.AreEqual( 5 , packages.Length );
	        byte index = 0;
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
                }                
	        }
	    }

	    [TestMethod]
	    public void TestPackageRectifierOutOfOrder()
	    {
            var package1 = new SegmentPackage(Config.PackageSize);
	        package1.SetSeq(0);            
	        package1.WritePayload(new byte[] {1}, 0, 1);

            var package2 = new SegmentPackage(Config.PackageSize);
	        package2.SetSeq(1);
	        package2.WritePayload(new byte[] { 5 }, 0, 1);            

	        var package3 = new SegmentPackage(Config.PackageSize);
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

	    [TestMethod]
	    public void TestPackageRectifierRepeat()
	    {
	        var package1 = new SegmentPackage(Config.PackageSize);
	        package1.SetSeq(0);
	        package1.WritePayload(new byte[] { 1 }, 0, 1);

	        var package2 = new SegmentPackage(Config.PackageSize);
	        package2.SetSeq(1);
	        package2.WritePayload(new byte[] { 5 }, 0, 1);

	        var package3 = new SegmentPackage(Config.PackageSize);
	        package3.SetSeq(2);
	        package3.WritePayload(new byte[] { 9 }, 0, 1);

            var package4 = new SegmentPackage(Config.PackageSize);
	        package4.SetSeq(3);
	        package4.WritePayload(new byte[] { 10 }, 0, 1);

	        var package5 = new SegmentPackage(Config.PackageSize);
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

        [TestMethod]
	    public void TestAck()
	    {
	        var package1 = new SegmentPackage(Config.PackageSize);
	        package1.SetSeq(1);
            var package2 = new SegmentPackage(Config.PackageSize);
	        package2.SetSeq(2);

            var ackWaiter = new CongestionRecorder(3);
	        ackWaiter.PushWait(package1, System.TimeSpan.FromSeconds(0.1).Ticks);
	        ackWaiter.PushWait(package2, System.TimeSpan.FromSeconds(1.0).Ticks);
            ackWaiter.Reply(package1.GetSeq());

            
	        var packages = ackWaiter.PopLost(System.TimeSpan.FromSeconds(1.0).Ticks);

            Assert.AreEqual(2u , packages[0].GetSeq());


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
