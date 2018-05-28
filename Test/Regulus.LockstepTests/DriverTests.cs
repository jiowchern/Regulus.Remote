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
    public class DriverTests
    {
        [Test()]
        public void DriveTest()
        {
            var driver = new Regulus.Lockstep.Driver<int>(1000,3);
            driver.Push(1);
            int command;
            bool frame;
            driver.Drive(1000, out frame , out command  );

            Assert.AreEqual(true ,frame);
            Assert.AreEqual(0, command);
        }

        [Test()]
        public void DriveKeyFrameTest1()
        {
            var driver = new Regulus.Lockstep.Driver<int>(1000, 3);
            driver.Push(1);
            int command;
            bool frame;
            driver.Drive(1000, out frame, out command);
            Assert.AreEqual(true, frame);
            Assert.AreEqual(0, command);

            driver.Drive(1000, out frame, out command);
            Assert.AreEqual(true, frame);
            Assert.AreEqual(0, command);

            driver.Drive(1000, out frame, out command);
            Assert.AreEqual(true, frame);
            Assert.AreEqual(1, command);
        }

        [Test()]
        public void DriveKeyFrameTest2()
        {
            var driver = new Regulus.Lockstep.Driver<int>(1000, 3);
            driver.Push(1);
            int command;
            bool frame;
            driver.Drive(999, out frame, out command);
            Assert.AreEqual(false, frame);
            Assert.AreEqual(0, command);

            driver.Drive(1001, out frame, out command);
            Assert.AreEqual(true, frame);
            Assert.AreEqual(0, command);

            driver.Drive(0, out frame, out command);
            Assert.AreEqual(true, frame);
            Assert.AreEqual(0, command);

            driver.Drive(0, out frame, out command);
            Assert.AreEqual(false, frame);
            Assert.AreEqual(0, command);


        }
    }
}