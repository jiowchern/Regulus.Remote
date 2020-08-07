using NUnit.Framework;

namespace Regulus.Lockstep.Tests
{
    [TestFixture()]
    public class PropellerTests
    {
        [Test()]
        public void PropellerAdvance1Test()
        {
            Propeller<int> propeller = new Propeller<int>(1000, 3, 1);

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
            Propeller<int> propeller = new Propeller<int>(1000, 3, 1);

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
            Propeller<int> propeller = new Propeller<int>(100, 3, 1);
            propeller.Push(1);
            propeller.Push(2);

            int step;
            Assert.AreEqual(true, propeller.Advance(1, out step));
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
            Propeller<int> propeller = new Propeller<int>(1000, 3, 1);
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
            Propeller<int> propeller = new Propeller<int>(1000, 3, 1);
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
            Propeller<int> propeller = new Propeller<int>(1000, 3, 1);
            propeller.Push(1);

            int step;


            Assert.AreEqual(false, propeller.Advance(999, out step));



        }


        [Test()]
        public void PropellerChase()
        {

            Propeller<string> propeller = new Propeller<string>(1, 3, 3);

            propeller.Push("command");
            propeller.Push("command");
            propeller.Push("command");
            propeller.Push("command");

            string step;
            Assert.AreEqual(true, propeller.Advance(0, out step));
            Assert.AreEqual(true, propeller.Advance(0, out step));
            Assert.AreEqual(true, propeller.Advance(0, out step));

            Assert.AreEqual(true, propeller.Advance(0, out step));
            Assert.AreEqual(true, propeller.Advance(0, out step));
            Assert.AreEqual(true, propeller.Advance(0, out step));

            Assert.AreEqual(true, propeller.Advance(0, out step));
            Assert.AreEqual(true, propeller.Advance(0, out step));
            Assert.AreEqual(true, propeller.Advance(0, out step));

            Assert.AreEqual(false, propeller.Advance(0, out step));
            Assert.AreEqual(false, propeller.Advance(0, out step));
            Assert.AreEqual(false, propeller.Advance(0, out step));

        }
    }
}

