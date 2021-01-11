using Xunit;

namespace RemotingTest
{
    public class Property
    {
        [Xunit.Fact]
        public void Equip()
        {
            Regulus.Remote.Property<int> p1 = new Regulus.Remote.Property<int>(1);
            Regulus.Remote.Property<int> p2 = new Regulus.Remote.Property<int>(1);

            bool result = p1 == p2;
            Assert.Equal(true, result);
        }
    }

}
