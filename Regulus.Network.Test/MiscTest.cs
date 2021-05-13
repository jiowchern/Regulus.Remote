using Xunit;
using Regulus.Network.Package;
using System.Collections.Generic;
using System.Net;

namespace Regulus.Network.Tests
{

    public class MiscTest
    {








        [Xunit.Fact]
        public void TestDataPackageSize()
        {
            Assert.True(Config.Default.PackageSize - SocketMessage.GetHeadSize() > 0);
        }


        [Xunit.Fact]
        public async void DirectWaitableValueTest()
        {
            IWaitableValue<int> dir = new NoWaitValue<int>(1)  ;
            var val = await dir;

            Assert.Equal(1 , val);
        }





        [Xunit.Fact]
        public void TestBufferDispenser1()
        {

            int count = SocketMessage.GetPayloadSize() * 4;
            TestMessage message = new TestMessage(count);


            BufferDispenser dispenser = new BufferDispenser(new IPEndPoint(IPAddress.Any, 0), SocketMessageFactory.Instance);
            SocketMessage[] packages = dispenser.PackingTransmission(message.Buffer, 0, 0);


            Assert.Equal(4, packages.Length);
            byte index = 0;
            int readcount = 0;
            for (uint i = 0; i < packages.Length; i++)
            {
                SocketMessage package = packages[i];
                Assert.Equal(i, package.GetSeq());

                List<byte> data = new List<byte>();
                package.ReadPayload(data);

                for (int j = 0; j < data.Count; j++)
                {
                    Assert.Equal(index, data[j]);
                    index++;
                    readcount++;
                }
            }

            Assert.Equal(readcount, count);
        }

        [Xunit.Fact]
        public void TestBufferDispenser2()
        {
            int count = SocketMessage.GetPayloadSize() * 4 + 1;
            TestMessage message = new TestMessage(count);


            BufferDispenser dispenser = new BufferDispenser(new IPEndPoint(IPAddress.Any, 0), SocketMessageFactory.Instance);
            SocketMessage[] packages = dispenser.PackingTransmission(message.Buffer, 0, 0);


            Assert.Equal(5, packages.Length);
            byte index = 0;
            int readcount = 0;
            for (uint i = 0; i < packages.Length; i++)
            {
                SocketMessage package = packages[i];
                Assert.Equal(i, package.GetSeq());

                List<byte> data = new List<byte>();
                package.ReadPayload(data);

                for (int j = 0; j < data.Count; j++)
                {
                    Assert.Equal(index, data[j]);
                    index++;
                    readcount++;

                }
            }

            Assert.Equal(readcount, count);
        }

        [Xunit.Fact]
        public void TestPackageRectifierOutOfOrder()
        {
            SocketMessage package1 = new SocketMessage(Config.Default.PackageSize);
            package1.SetSeq(0);
            package1.WritePayload(new byte[] { 1 }, 0, 1);

            SocketMessage package2 = new SocketMessage(Config.Default.PackageSize);
            package2.SetSeq(1);
            package2.WritePayload(new byte[] { 5 }, 0, 1);

            SocketMessage package3 = new SocketMessage(Config.Default.PackageSize);
            package3.SetSeq(2);
            package3.WritePayload(new byte[] { 9 }, 0, 1);


            PackageRectifier receiver = new PackageRectifier();
            receiver.PushPackage(package3);

            SocketMessage stream1 = receiver.PopPackage();
            Assert.Equal(null, stream1);

            receiver.PushPackage(package2);


            SocketMessage stream2 = receiver.PopPackage();
            Assert.Equal(null, stream2);


            receiver.PushPackage(package1);

            Assert.Equal((byte)1, receiver.PopPackage().ReadPayload(0));
            Assert.Equal((byte)5, receiver.PopPackage().ReadPayload(0));
            Assert.Equal((byte)9, receiver.PopPackage().ReadPayload(0));

        }

        [Xunit.Fact]
        public void TestPackageRectifierRepeat()
        {
            SocketMessage package1 = new SocketMessage(Config.Default.PackageSize);
            package1.SetSeq(0);
            package1.WritePayload(new byte[] { 1 }, 0, 1);

            SocketMessage package2 = new SocketMessage(Config.Default.PackageSize);
            package2.SetSeq(1);
            package2.WritePayload(new byte[] { 5 }, 0, 1);

            SocketMessage package3 = new SocketMessage(Config.Default.PackageSize);
            package3.SetSeq(2);
            package3.WritePayload(new byte[] { 9 }, 0, 1);

            SocketMessage package4 = new SocketMessage(Config.Default.PackageSize);
            package4.SetSeq(3);
            package4.WritePayload(new byte[] { 10 }, 0, 1);

            SocketMessage package5 = new SocketMessage(Config.Default.PackageSize);
            package5.SetSeq(4);
            package5.WritePayload(new byte[] { 11 }, 0, 1);


            PackageRectifier receiver = new PackageRectifier();
            receiver.PushPackage(package3);


            SocketMessage stream1 = receiver.PopPackage();
            Assert.Equal(null, stream1);

            receiver.PushPackage(package2);
            SocketMessage stream2 = receiver.PopPackage();
            Assert.Equal(null, stream2);

            receiver.PushPackage(package1);


            Assert.Equal((byte)1, receiver.PopPackage().ReadPayload(0));
            Assert.Equal((byte)5, receiver.PopPackage().ReadPayload(0));
            Assert.Equal((byte)9, receiver.PopPackage().ReadPayload(0));


            receiver.PushPackage(package5);
            SocketMessage stream4 = receiver.PopPackage();
            Assert.Equal(null, stream4);

            receiver.PushPackage(package2);
            SocketMessage stream5 = receiver.PopPackage();
            Assert.Equal(null, stream5);

            receiver.PushPackage(package4);

            Assert.Equal((byte)10, receiver.PopPackage().ReadPayload(0));
            Assert.Equal((byte)11, receiver.PopPackage().ReadPayload(0));

        }

        [Xunit.Fact]
        public void TestAck1()
        {
            SocketMessage package1 = new SocketMessage(Config.Default.PackageSize);
            package1.SetSeq(1);
            SocketMessage package2 = new SocketMessage(Config.Default.PackageSize);
            package2.SetSeq(2);

            CongestionRecorder ackWaiter = new CongestionRecorder(3);


            ackWaiter.PushWait(package1, (long)(Timestamp.OneSecondTicks * 0.1));
            ackWaiter.PushWait(package2, Timestamp.OneSecondTicks);
            ackWaiter.Reply(package1.GetSeq(), Timestamp.OneSecondTicks, 1);
            List<SocketMessage> packages = ackWaiter.PopLost(Timestamp.OneSecondTicks * 100, Timestamp.OneSecondTicks * 100);

            Assert.Equal(2u, packages[0].GetSeq());


        }

        [Xunit.Fact]
        public void TestAck2()
        {
            SocketMessage package1 = new SocketMessage(Config.Default.PackageSize);
            package1.SetSeq(0);
            SocketMessage package2 = new SocketMessage(Config.Default.PackageSize);
            package2.SetSeq(1);
            SocketMessage package3 = new SocketMessage(Config.Default.PackageSize);
            package3.SetSeq(2);

            CongestionRecorder ackWaiter = new CongestionRecorder(3);
            PackageRectifier rectifier = new PackageRectifier();

            ackWaiter.PushWait(package1, 1);
            ackWaiter.PushWait(package2, 2);
            ackWaiter.PushWait(package3, 3);

            rectifier.PushPackage(package3);
            rectifier.PushPackage(package1);


            ackWaiter.ReplyBefore((ushort)(rectifier.Serial - 1), 1, 1);
            ackWaiter.ReplyAfter((ushort)(rectifier.Serial - 1), rectifier.SerialBitFields, 1, 1);

            Assert.Equal(1, ackWaiter.Count);

            List<SocketMessage> outs = ackWaiter.PopLost(Timestamp.OneSecondTicks * 60, Timestamp.OneSecondTicks * 60);

            Assert.Equal(1, outs[0].GetSeq());




        }





    }

    public class TestSpawner : IObjectProvider<SocketMessage>
    {
        SocketMessage IObjectProvider<SocketMessage>.Spawn()
        {
            return new SocketMessage(50);
        }
    }

    public class ByteArrayFactory : IObjectProvider<byte[]>
    {
        byte[] IObjectProvider<byte[]>.Spawn()
        {
            return new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
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
