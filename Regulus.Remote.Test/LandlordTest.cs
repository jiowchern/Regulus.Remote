using NUnit.Framework;

namespace Regulus.Remote.Tests
{
    public class LandlordTest
    {
        [NUnit.Framework.Test()]
        public void LongTest()
        {
            var landlord = new Landlord<long>(new LongProvider());
            var l1 = landlord.Rent();
            var l2 = landlord.Rent();            
            landlord.Return(l2);
            var l3 = landlord.Rent();
            Assert.AreEqual(3 , l1);
            Assert.AreEqual(4, l2);
            Assert.AreEqual(4, l3);
        }
    }
}