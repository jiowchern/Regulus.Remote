using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Network.RUDP;

namespace Regulus.Network.Tests
{
    [TestClass]
    public class PacketTest
    {
        [TestMethod]
        public void Seq()
        {
            var buffer = new byte[Config.PackageSize];
            buffer.SetSeq(0x1234);
            var seq = buffer.GetSeq();
            Assert.AreEqual((ushort)0x1234, seq.Value);
        }

        [TestMethod]
        public void Ack()
        {
            var buffer = new byte[Config.PackageSize];
            buffer.SetAck(0x1234);
            var value = buffer.GetAck();
            Assert.AreEqual((ushort)0x1234, value.Value);
        }
    }
}
