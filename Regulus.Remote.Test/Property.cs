using NUnit.Framework;

namespace RemotingTest
{
    public class Property
    {
        [NUnit.Framework.Test()]
        public void Equip()
        {
            Regulus.Remote.Property<int> p1 = new Regulus.Remote.Property<int>(1);
            Regulus.Remote.Property<int> p2 = new Regulus.Remote.Property<int>(1);

            bool result = p1 == p2;
            Assert.AreEqual(true, result);
        }
    }

}
