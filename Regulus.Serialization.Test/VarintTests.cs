using System;
using System.Linq;



namespace Regulus.Serialization.Tests
{
    public class VarintTests
    {

        [NUnit.Framework.Test]
        public void FindVarintTest()
        {
            var buffer = new byte[] {0x90 , 0xf8 , 0x01 , 0x02 ,0xdf};

            var seg = new ArraySegment<byte>(buffer, 0, buffer.Length);
            var varint = Varint.FindVarint(ref seg);

            NUnit.Framework.Assert.AreEqual(3, varint.Count);
        }
        [NUnit.Framework.Test]

        public void FindPackagesTest()
        {
            var buffer = new byte[] {0x00, 0x02, 0xf8, 0xf8, 0x01, 0xdf ,0x8c };

            var seg = new ArraySegment<byte>(buffer, 1, buffer.Length-1);
            var varintPairs = Varint.FindPackages(seg).ToArray();

            NUnit.Framework.Assert.AreEqual(1, varintPairs[0].Head.Offset );
            NUnit.Framework.Assert.AreEqual(1, varintPairs[0].Head.Count);
            NUnit.Framework.Assert.AreEqual(2, varintPairs[0].Body.Offset);
            NUnit.Framework.Assert.AreEqual(2, varintPairs[0].Body.Count);

            NUnit.Framework.Assert.AreEqual(4, varintPairs[1].Head.Offset);
            NUnit.Framework.Assert.AreEqual(1, varintPairs[1].Head.Count);
            NUnit.Framework.Assert.AreEqual(5, varintPairs[1].Body.Offset);
            NUnit.Framework.Assert.AreEqual(1, varintPairs[1].Body.Count);
        }
        [NUnit.Framework.Test]
        public void FindPackagesTest2()
        {
            var buffer = new byte[] { 0x02, 0x01 };

            var seg = new ArraySegment<byte>(buffer, 0, buffer.Length);
            var varintPairs = Varint.FindPackages(seg);

            NUnit.Framework.Assert.AreEqual(0, varintPairs.Count());
            
        }
    }

}



