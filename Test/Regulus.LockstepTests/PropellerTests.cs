using NUnit.Framework;
using Regulus.Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Lockstep.Tests
{
    [TestFixture()]
    public class PropellerTests
    {
        [Test()]
        public void PropellerAdvance1Test()
        {
            var propeller = new Propeller<int>(1000 , 3);

            propeller.Push(1);

            int step;
            if (!propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }
            Assert.AreEqual(0, step);
            if (!propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }
            Assert.AreEqual(0, step);
            if (!propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }

            Assert.AreEqual(1, step);
        }


        [Test()]
        public void PropellerAdvance2Test()
        {
            var propeller = new Propeller<int>(1000, 3);

            propeller.Push(1);

            int step;
            if (!propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }
            Assert.AreEqual(0, step);
            if (!propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }
            Assert.AreEqual(0, step);
            if (!propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }

            Assert.AreEqual(1, step);


            propeller.Push(2);

            
            if (!propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }
            Assert.AreEqual(0, step);
            if (!propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }
            Assert.AreEqual(0, step);
            if (!propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }

            Assert.AreEqual(2, step);
        }

        [Test()]
        public void PropellerAdvanceSeek1Test()
        {
            var propeller = new Propeller<int>(100, 3);
            propeller.Push(1);
            propeller.Push(2);

            int step;
            Assert.AreEqual(true , propeller.Advance(1, out step));
            Assert.AreEqual(0, step);
            
            Assert.AreEqual(true, propeller.Advance(1, out step));
            Assert.AreEqual(0, step);

            Assert.AreEqual(true, propeller.Advance(1, out step));
            Assert.AreEqual(1, step);


            Assert.AreEqual(true, propeller.Advance(300, out step));
            Assert.AreEqual(0, step);

            Assert.AreEqual(true, propeller.Advance(1, out step));
            Assert.AreEqual(0, step);

            Assert.AreEqual(true, propeller.Advance(1, out step));
            Assert.AreEqual(2, step);



        }

        [Test()]
        public void PropellerAdvanceEmpty1Test()
        {
            var propeller = new Propeller<int>(1000, 3);
            int step;
            if (propeller.Advance(1000, out step))
            {
                Assert.Fail();
            }

            Assert.AreEqual(false, propeller.Advance(1000, out step));
        }


        [Test()]
        public void PropellerAdvanceEmpty2Test()
        {
            var propeller = new Propeller<int>(1000, 3);
            propeller.Push(1);

            int step;
            propeller.Advance(1000, out step);
            propeller.Advance(1000, out step);
            propeller.Advance(1000, out step);
            Assert.AreEqual(false, propeller.Advance(1000, out step)); 



        }

        [Test()]
        public void PropellerAdvanceInsufficienTest()
        {
            var propeller = new Propeller<int>(1000, 3);
            propeller.Push(1);

            int step;
            
            
            Assert.AreEqual(false, propeller.Advance(999, out step));



        }
    }
}

