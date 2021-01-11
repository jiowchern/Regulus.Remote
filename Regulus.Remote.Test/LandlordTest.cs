using Xunit;

namespace Regulus.Remote.Tests
{
    public class LandlordTest
    {
        [Xunit.Fact]
        public void LongTest()
        {
            Landlord<long> landlord = new Landlord<long>(new LongProvider());
            long l1 = landlord.Rent();
            long l2 = landlord.Rent();
            landlord.Return(l2);
            long l3 = landlord.Rent();
            Assert.Equal(1, l1);
            Assert.Equal(2, l2);
            Assert.Equal(2, l3);
        }
    }
}