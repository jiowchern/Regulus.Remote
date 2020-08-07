using NUnit.Framework;

namespace Regulus.Remote.Tests
{
    public class LandlordTest
    {
        [NUnit.Framework.Test()]
        public void LongTest()
        {
            Landlord<long> landlord = new Landlord<long>(new LongProvider());
            long l1 = landlord.Rent();
            long l2 = landlord.Rent();
            landlord.Return(l2);
            long l3 = landlord.Rent();
            Assert.AreEqual(3, l1);
            Assert.AreEqual(4, l2);
            Assert.AreEqual(4, l3);
        }
    }
}