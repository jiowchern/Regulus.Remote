using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace Regulus.Lockstep.Tests
{
    [TestFixture()]
    public class DriverTests
    {
        [Test()]
        public void DriverEmptyTest()
        {
            ICommandProvidable<int> provider = NSubstitute.Substitute.For<ICommandProvidable<int>>();
            provider.Current.Returns(1);
            Driver<int> driver = new Driver<int>(1000);
            IPlayer<int> player = driver.Regist(provider);
            driver.Advance(0);
            int stepCount = player.PopSteps().Count();
            Assert.AreEqual(0, stepCount);


            Assert.IsTrue(driver.Unregist(player));
        }

        [Test()]
        public void DriverTest()
        {
            ICommandProvidable<int> provider = NSubstitute.Substitute.For<ICommandProvidable<int>>();
            provider.Current.Returns(1);
            Driver<int> driver = new Driver<int>(1000);
            IPlayer<int> player = driver.Regist(provider);
            driver.Advance(1000);
            Step<Driver<int>.Record> step = player.PopSteps().First();
            Assert.AreEqual(1, step.Records[0].Command);


            Assert.IsTrue(driver.Unregist(player));
        }


        [Test()]
        public void DriverRecoverTest()
        {
            ICommandProvidable<int> provider1 = NSubstitute.Substitute.For<ICommandProvidable<int>>();
            ICommandProvidable<int> provider2 = NSubstitute.Substitute.For<ICommandProvidable<int>>();
            provider1.Current.Returns(1);
            provider2.Current.Returns(2);
            Driver<int> driver = new Driver<int>(1);
            IPlayer<int> player1 = driver.Regist(provider1);
            driver.Advance(1);

            IPlayer<int> player2 = driver.Regist(provider2);
            Step<Driver<int>.Record>[] steps = player2.PopSteps().ToArray();
            Assert.AreEqual(1, steps[0].Records[0].Command);

            Assert.IsTrue(driver.Unregist(player1));
            Assert.IsTrue(driver.Unregist(player2));
        }


    }
}