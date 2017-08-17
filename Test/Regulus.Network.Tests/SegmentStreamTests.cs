
using Regulus.Network.RUDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Regulus.Network.RUDP.Tests
{
    
    public class SegmentStreamTests
    {
        [Test]
        public void FullReadTest()
        {
            var pkg1 = new SocketMessage(SocketMessage.HEADSIZE + 5);
            var pkg2 = new SocketMessage(SocketMessage.HEADSIZE + 4);
            var pkg3 = new SocketMessage(SocketMessage.HEADSIZE + 2);

            pkg1.WritePayload(new byte[] { 0, 1, 2, 3, 4}, 0, 5);
            pkg2.WritePayload(new byte[] { 5,6,7,8 }, 0, 4);
            pkg3.WritePayload(new byte[] { 9,10 }, 0, 2);

            var stream = new SegmentStream(new []{ pkg1  , pkg2 , pkg3});
            var readBuffer = new byte[15] {255,255, 255, 255 , 255, 255 , 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            stream.Read(readBuffer, 2, 13);

            Assert.AreEqual(255,readBuffer[0] );
            Assert.AreEqual(255, readBuffer[1]);
            Assert.AreEqual(0, readBuffer[2]);
            Assert.AreEqual(1, readBuffer[3]);
            Assert.AreEqual(2, readBuffer[4]);
            Assert.AreEqual(3, readBuffer[5]);
            Assert.AreEqual(4, readBuffer[6]);
            Assert.AreEqual(5, readBuffer[7]);
            Assert.AreEqual(6, readBuffer[8]);
            Assert.AreEqual(7, readBuffer[9]);
            Assert.AreEqual(8, readBuffer[10]);
            Assert.AreEqual(9, readBuffer[11]);
            Assert.AreEqual(10, readBuffer[12]);
            Assert.AreEqual(255, readBuffer[13]);
            Assert.AreEqual(255, readBuffer[14]);            

        }

        [Test]
        public void BatchesReadTest()
        {
            var pkg1 = new SocketMessage(SocketMessage.HEADSIZE + 5);
            var pkg2 = new SocketMessage(SocketMessage.HEADSIZE + 4);
            var pkg3 = new SocketMessage(SocketMessage.HEADSIZE + 2);

            pkg1.WritePayload(new byte[] { 0, 1, 2, 3, 4 }, 0, 5);
            pkg2.WritePayload(new byte[] { 5, 6, 7, 8 }, 0, 4);
            pkg3.WritePayload(new byte[] { 9, 10 }, 0, 2);

            var stream = new SegmentStream(new[] { pkg1, pkg2, pkg3 });
            var readBuffer = new byte[8] { 255, 255, 255, 255, 255, 255, 255, 255 };
            stream.Read(readBuffer, 2, 6);

            Assert.AreEqual(255, readBuffer[0]);
            Assert.AreEqual(255, readBuffer[1]);
            Assert.AreEqual(0, readBuffer[2]);
            Assert.AreEqual(1, readBuffer[3]);
            Assert.AreEqual(2, readBuffer[4]);
            Assert.AreEqual(3, readBuffer[5]);
            Assert.AreEqual(4, readBuffer[6]);
            Assert.AreEqual(5, readBuffer[7]);

            readBuffer = new byte[8] { 255, 255, 255, 255, 255, 255, 255, 255 };
            int readCount = stream.Read(readBuffer, 0, 6);

            Assert.AreEqual(5, readCount);

            Assert.AreEqual(6, readBuffer[0]);
            Assert.AreEqual(7, readBuffer[1]);
            Assert.AreEqual(8, readBuffer[2]);
            Assert.AreEqual(9, readBuffer[3]);
            Assert.AreEqual(10, readBuffer[4]);
            Assert.AreEqual(255, readBuffer[5]);
            Assert.AreEqual(255, readBuffer[6]);
            Assert.AreEqual(255, readBuffer[7]);

        }
    }
}