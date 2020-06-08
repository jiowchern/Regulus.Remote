using NUnit.Framework;

namespace RemotingTest
{
    public class Property
	{
		[NUnit.Framework.Test()]
		public void Equip()
		{
			var p1 = new Regulus.Remote.Property<int>(1);
			var p2 = new Regulus.Remote.Property<int>(1);

			var result = p1 == p2;
			Assert.AreEqual(true , result);
		}
	}

}
