using Xunit;
using Regulus.Network.Package;

namespace Regulus.Network.RUDP.Tests
{

    public class SegmentStreamTests
    {
        [Xunit.Fact]
        public void FullReadTest()
        {

            SocketMessage pkg1 = new SocketMessage(Regulus.Network.Config.Default.PackageSize + 5);
            SocketMessage pkg2 = new SocketMessage(Regulus.Network.Config.Default.PackageSize + 4);
            SocketMessage pkg3 = new SocketMessage(Regulus.Network.Config.Default.PackageSize + 2);

            pkg1.WritePayload(new byte[] { 0, 1, 2, 3, 4 }, 0, 5);
            pkg2.WritePayload(new byte[] { 5, 6, 7, 8 }, 0, 4);
            pkg3.WritePayload(new byte[] { 9, 10 }, 0, 2);

            SegmentStream stream = new SegmentStream(new[] { pkg1, pkg2, pkg3 });
            byte[] readBuffer = new byte[15] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            stream.Read(readBuffer, 2, 13);

            Assert.Equal(255, readBuffer[0]);
            Assert.Equal(255, readBuffer[1]);
            Assert.Equal(0, readBuffer[2]);
            Assert.Equal(1, readBuffer[3]);
            Assert.Equal(2, readBuffer[4]);
            Assert.Equal(3, readBuffer[5]);
            Assert.Equal(4, readBuffer[6]);
            Assert.Equal(5, readBuffer[7]);
            Assert.Equal(6, readBuffer[8]);
            Assert.Equal(7, readBuffer[9]);
            Assert.Equal(8, readBuffer[10]);
            Assert.Equal(9, readBuffer[11]);
            Assert.Equal(10, readBuffer[12]);
            Assert.Equal(255, readBuffer[13]);
            Assert.Equal(255, readBuffer[14]);

        }

        [Xunit.Fact]
        public void BatchesReadTest()
        {
            SocketMessage pkg1 = new SocketMessage(Regulus.Network.Config.Default.PackageSize + 5);
            SocketMessage pkg2 = new SocketMessage(Regulus.Network.Config.Default.PackageSize + 4);
            SocketMessage pkg3 = new SocketMessage(Regulus.Network.Config.Default.PackageSize + 2);

            pkg1.WritePayload(new byte[] { 0, 1, 2, 3, 4 }, 0, 5);
            pkg2.WritePayload(new byte[] { 5, 6, 7, 8 }, 0, 4);
            pkg3.WritePayload(new byte[] { 9, 10 }, 0, 2);

            SegmentStream stream = new SegmentStream(new[] { pkg1, pkg2, pkg3 });
            byte[] readBuffer = new byte[8] { 255, 255, 255, 255, 255, 255, 255, 255 };
            stream.Read(readBuffer, 2, 6);

            Assert.Equal(255, readBuffer[0]);
            Assert.Equal(255, readBuffer[1]);
            Assert.Equal(0, readBuffer[2]);
            Assert.Equal(1, readBuffer[3]);
            Assert.Equal(2, readBuffer[4]);
            Assert.Equal(3, readBuffer[5]);
            Assert.Equal(4, readBuffer[6]);
            Assert.Equal(5, readBuffer[7]);

            readBuffer = new byte[8] { 255, 255, 255, 255, 255, 255, 255, 255 };
            int readCount = stream.Read(readBuffer, 0, 6);

            Assert.Equal(5, readCount);

            Assert.Equal(6, readBuffer[0]);
            Assert.Equal(7, readBuffer[1]);
            Assert.Equal(8, readBuffer[2]);
            Assert.Equal(9, readBuffer[3]);
            Assert.Equal(10, readBuffer[4]);
            Assert.Equal(255, readBuffer[5]);
            Assert.Equal(255, readBuffer[6]);
            Assert.Equal(255, readBuffer[7]);

        }


    }


}