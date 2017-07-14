using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Network.RUDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Network.RUDP.Tests
{
    [TestClass()]
    public class SegmentStreamTests
    {
        [TestMethod()]
        public void ReadTest1()
        {
            var pkg1 = new SegmentPackage(SegmentPackage.HEADSIZE + 5);
            var pkg2 = new SegmentPackage(SegmentPackage.HEADSIZE + 4);
            var pkg3 = new SegmentPackage(SegmentPackage.HEADSIZE + 2);

            pkg1.WritePayload(new byte[] {0, 1, 2, 3, 4}, 0, 5);
            pkg2.WritePayload(new byte[] { 5,6,7,8 }, 0, 4);
            pkg3.WritePayload(new byte[] { 9,10 }, 0, 2);

            var stream = new SegmentStream(new []{ pkg1  , pkg2 , pkg3});
            var readBuffer = new byte[8] {255,255, 255, 255 , 255, 255 , 255, 255 };
            stream.Read(4, readBuffer, 2, 6);

            Assert.AreEqual(255,readBuffer[0] );
            Assert.AreEqual(255, readBuffer[1]);
            Assert.AreEqual(4, readBuffer[2]);
            Assert.AreEqual(5, readBuffer[3]);
            Assert.AreEqual(6, readBuffer[4]);
            Assert.AreEqual(7, readBuffer[5]);
            Assert.AreEqual(8, readBuffer[6]);
            Assert.AreEqual(9, readBuffer[7]);
            
        }

        [TestMethod()]
        public void ReadTest2()
        {
            var pkg1 = new SegmentPackage(SegmentPackage.HEADSIZE + 5);
            var pkg2 = new SegmentPackage(SegmentPackage.HEADSIZE + 4);
            var pkg3 = new SegmentPackage(SegmentPackage.HEADSIZE + 2);

            pkg1.WritePayload(new byte[] { 0, 1, 2, 3, 4 }, 0, 5);
            pkg2.WritePayload(new byte[] { 5, 6, 7, 8 }, 0, 4);
            pkg3.WritePayload(new byte[] { 9, 10 }, 0, 2);

            var stream = new SegmentStream(new[] { pkg1, pkg2, pkg3 });
            var readBuffer = new byte[8] { 255, 255, 255, 255, 255, 255, 255, 255 };
            stream.Read(5, readBuffer, 2, 6);

            Assert.AreEqual(255, readBuffer[0]);
            Assert.AreEqual(255, readBuffer[1]);
            Assert.AreEqual(5, readBuffer[2]);
            Assert.AreEqual(6, readBuffer[3]);
            Assert.AreEqual(7, readBuffer[4]);
            Assert.AreEqual(8, readBuffer[5]);
            Assert.AreEqual(9, readBuffer[6]);
            Assert.AreEqual(10, readBuffer[7]);

        }
    }
}