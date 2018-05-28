using NUnit.Framework;
using Regulus.Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Lockstep.Tests
{
    [TestFixture()]
    public class PropellerTests
    {
        [Test()]
        public void PropelTest1()
        {
            var propeller = new Regulus.Lockstep.Propeller(1000);
            propeller.Heartbeat();
            var step1 = propeller.Propel(1000);
            var step2 = propeller.Propel(1000);
            Assert.AreEqual(true, step1);
            Assert.AreEqual(false, step2);
        }

        [Test()]
        public void PropelTest2()
        {
            var propeller = new Regulus.Lockstep.Propeller(1000);
            propeller.Heartbeat();
            propeller.Heartbeat();

            var step1 = propeller.Propel(1000);
            var step2 = propeller.Propel(1000);
            Assert.AreEqual(true, step1);
            Assert.AreEqual(true, step2);
        }

        [Test()]
        public void PropelTest3()
        {
            var propeller = new Regulus.Lockstep.Propeller(1000);
            propeller.Heartbeat();


            var step1 = propeller.Propel(1000);

            propeller.Heartbeat();

            var step2 = propeller.Propel(1000);
            Assert.AreEqual(true, step1);
            Assert.AreEqual(true, step2);
        }

        [Test()]
        public void PropelTest4()
        {
            var propeller = new Regulus.Lockstep.Propeller(1000);
            


            var step1 = propeller.Propel(1000);
            var step2 = propeller.Propel(1000);
            var step3 = propeller.Propel(1000);

            propeller.Heartbeat();

            var step4 = propeller.Propel(1000);


            Assert.AreEqual(false, step1);
            Assert.AreEqual(false, step2);
            Assert.AreEqual(false, step3);
            Assert.AreEqual(true, step4);
        }
    }
}