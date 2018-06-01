using NUnit.Framework;
using Regulus.Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;

namespace Regulus.Lockstep.Tests
{
    [TestFixture()]
    public class DriverTests
    {
        [Test()]
        public void DriverEmptyTest()
        {
            var provider = NSubstitute.Substitute.For<ICommandProvidable<int>>();
            provider.Current.Returns(1);
            var driver = new Driver<int>(1000);
            var player = driver.Regist(provider);
            driver.Advance(0);
            var stepCount = player.PopSteps().Count();
            Assert.AreEqual(0, stepCount);


            Assert.IsTrue(driver.Unregist(player));
        }

        [Test()]
        public void DriverTest()
        {
            var provider = NSubstitute.Substitute.For<ICommandProvidable<int>>();
            provider.Current.Returns(1);
            var driver = new Driver<int>(1000);
            var player = driver.Regist(provider);
            driver.Advance(1000);
            var step = player.PopSteps().First();
            Assert.AreEqual(1 , step.Records[0].Command);


            Assert.IsTrue(driver.Unregist(player));
        }


        [Test()]
        public void DriverRecoverTest()
        {
            var provider1 = NSubstitute.Substitute.For<ICommandProvidable<int>>();
            var provider2 = NSubstitute.Substitute.For<ICommandProvidable<int>>();
            provider1.Current.Returns(1);
            provider2.Current.Returns(2);
            var driver = new Driver<int>(1);
            var player1 = driver.Regist(provider1);
            driver.Advance(1);

            var player2 = driver.Regist(provider2);
            var steps = player2.PopSteps().ToArray();
            Assert.AreEqual(1, steps[0].Records[0].Command);

            Assert.IsTrue(driver.Unregist(player1));
            Assert.IsTrue(driver.Unregist(player2));
        }


    }
}