using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Regulus.Network.RUDP;

namespace Regulus.Network.Tests
{
	[TestClass]
	public class UnitTest1
	{
        [TestMethod]
	    public void TestDataPackageSize()
	    {	        
            Assert.IsTrue(Peer.PackageSize - MessagePackage.GetHeadSize() > 0);	        
	    }

	    [TestMethod]
	    public void TestEmptyDataPackageSerialize()
	    {
	        var serializer = Peer.CreateSerializer();
	        var package = new MessagePackage();
	        package.Serial = 1;
	        package.Data = new byte[0];
            var buffer = serializer.ObjectToBuffer(package);
	        Assert.AreEqual(MessagePackage.GetHeadSize(), buffer.Length);


        }

	    

	    [TestMethod]
	    public void TestBufferDispenser()
	    {
	        var provider = NSubstitute.Substitute.For<ISerialProvider>();
	        provider.AllocateSerial(NSubstitute.Arg.Any<int>()).Returns(new uint[] {1, 2, 3 , 4});
            var message = new TestMessage(350);	        

            var dispenser = new BufferDispenser( 100);
	        var packages = dispenser.Packing(message.Buffer ,0,0);


            Assert.AreEqual( 4 , packages.Length );
	        byte index = 0;
	        for (uint i = 0; i < packages.Length; i++)
	        {
	            var package = packages[i];
	            Assert.AreEqual(i, package.Serial);
                
                var data = package.Data;
	            for (int j = 0; j < data.Length; j++)
	            {	                
	                Assert.AreEqual(index, data[j]);
	                index++;
                }                
	        }
	    }

	    [TestMethod]
	    public void TestPackageRectifier()
	    {
            var package1 = new MessagePackage();
	        package1.Serial = 0;
            package1.Data = new byte[] {1};

	        var package2 = new MessagePackage();
	        package2.Serial = 1;
            package2.Data = new byte[] {5};

	        var package3 = new MessagePackage();
	        package3.Serial = 2;
            package3.Data = new byte[] {9};


            var receiver = new PackageRectifier();
	        receiver.PushPackage(package3);
	        var stream1 = receiver.PopStream();
            Assert.AreEqual(0 , stream1.Length);

	        receiver.PushPackage(package2);
	        var stream2 = receiver.PopStream();
            Assert.AreEqual(0, stream2.Length);

	        receiver.PushPackage(package1);
	        var stream3 = receiver.PopStream();
            Assert.AreEqual(1, stream3[0]);
	        Assert.AreEqual(5, stream3[1]);
	        Assert.AreEqual(9, stream3[2]);

        }

	    [TestMethod]
	    public void TestAck()
	    {
	        var package1 = 1u;
	        var package2 = 2u;
	        
            var ackWaiter = new AckWaiter();
	        ackWaiter.PushWait(package1, System.TimeSpan.FromSeconds(0.0).Ticks);
	        ackWaiter.PushWait(package2, System.TimeSpan.FromSeconds(1.0).Ticks);
            ackWaiter.EraseReply(package1);

            List<uint> packages = new List<uint>();
            ackWaiter.PopTimeout(System.TimeSpan.FromSeconds(0.1).Ticks,ref packages);

            Assert.AreEqual(2u , packages[0]);


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
