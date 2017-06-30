﻿using System;
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
	    public void TestDataPackageSize()
	    {	        
            Assert.IsTrue(Config.PackageSize - SegmentPackage.GetHeadSize() > 0);	        
	    }

	    [TestMethod]
	    public void TestEmptyDataPackageSerialize()
	    {
	        var serializer = Line.CreateSerializer();
	        var package = new SegmentPackage();
	        package.Serial = 1;
	        package.Ack = 1;
	        package.AckBits = 1;
            package.Data = new byte[0];
            var buffer = serializer.ObjectToBuffer(package);
	        Assert.IsTrue( buffer.Length <= SegmentPackage.GetHeadSize());


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
	    public void TestPackageRectifierOutOfOrder()
	    {
            var package1 = new SegmentPackage();
	        package1.Serial = 0;
            package1.Data = new byte[] {1};

	        var package2 = new SegmentPackage();
	        package2.Serial = 1;
            package2.Data = new byte[] {5};

	        var package3 = new SegmentPackage();
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
	    public void TestPackageRectifierRepeat()
	    {
	        var package1 = new SegmentPackage();
	        package1.Serial = 0;
	        package1.Data = new byte[] { 1 };

	        var package2 = new SegmentPackage();
	        package2.Serial = 1;
	        package2.Data = new byte[] { 5 };

	        var package3 = new SegmentPackage();
	        package3.Serial = 2;
	        package3.Data = new byte[] { 9 };

	        var package4 = new SegmentPackage();
	        package4.Serial = 3;
	        package4.Data = new byte[] { 10 };

	        var package5 = new SegmentPackage();
	        package5.Serial = 4;
	        package5.Data = new byte[] { 11 };


            var receiver = new PackageRectifier();
	        receiver.PushPackage(package3);
	        var stream1 = receiver.PopStream();
	        Assert.AreEqual(0, stream1.Length);

	        receiver.PushPackage(package2);
	        var stream2 = receiver.PopStream();
	        Assert.AreEqual(0, stream2.Length);

	        receiver.PushPackage(package1);
	        var stream3 = receiver.PopStream();
	        Assert.AreEqual(1, stream3[0]);
	        Assert.AreEqual(5, stream3[1]);
	        Assert.AreEqual(9, stream3[2]);


	        receiver.PushPackage(package5);
	        var stream4 = receiver.PopStream();
	        Assert.AreEqual(0, stream4.Length);

	        receiver.PushPackage(package2);
	        var stream5 = receiver.PopStream();
	        Assert.AreEqual(0, stream5.Length);

            receiver.PushPackage(package4);
	        var stream6 = receiver.PopStream();
	        Assert.AreEqual(2, stream6.Length);
	        Assert.AreEqual(10, stream6[0]);
	        Assert.AreEqual(11, stream6[1]);

        }

        [TestMethod]
	    public void TestAck()
	    {
	        var package1 = new SegmentPackage();
	        package1.Serial = 1;
            var package2 = new SegmentPackage();
	        package2.Serial = 2;

            var ackWaiter = new CongestionRecorder(3);
	        ackWaiter.PushWait(package1, System.TimeSpan.FromSeconds(0.1).Ticks);
	        ackWaiter.PushWait(package2, System.TimeSpan.FromSeconds(1.0).Ticks);
            ackWaiter.Reply(package1.Serial);

            
	        var packages = ackWaiter.PopLost(System.TimeSpan.FromSeconds(1.0).Ticks);

            Assert.AreEqual(2u , packages[0].Serial);


	    }

	    [TestMethod]
	    public void TestAckBitOverflow()
	    {
	        var pkg = new SegmentPackage();
            /*
             * fa x
             * fb 0
             * fc 0
             * fd 0
             * fe 0
             * ff 0
             * 00 0
             * 01 1 0x40
             */
	        pkg.Ack = 0xfffffffa; 
	        pkg.SetAcks(0x1);

            Assert.AreEqual(0x40u , pkg.AckBits);
	    }

	    [TestMethod]
	    public void TestAckBit()
	    {
	        var pkg = new SegmentPackage();
	        
	        pkg.Ack = 1;
	        pkg.SetAcks(3);

	        Assert.AreEqual(0x2u, pkg.AckBits);
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
