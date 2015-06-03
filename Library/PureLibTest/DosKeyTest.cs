using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PureLibTest
{
    [TestClass]
    public class DosKeyTest
    {
        [TestMethod]
        public void TestPrevInit()
        {
            var dosKey = new Regulus.Utility.Doskey(10);

            string record1 = dosKey.TryGetPrev();

            Assert.AreEqual(null, record1);
        }
        [TestMethod]
        public void TestPrev()
        {
            var dosKey = new Regulus.Utility.Doskey(10);

            dosKey.Record("in1");
            dosKey.Record("in2");
            dosKey.Record("in3");
            string record1 = dosKey.TryGetPrev();
            string record2 = dosKey.TryGetPrev();
            string record3 = dosKey.TryGetPrev();
            string record4 = dosKey.TryGetPrev();

            Assert.AreEqual("in3", record1);
            Assert.AreEqual("in2", record2);
            Assert.AreEqual("in1", record3);
            Assert.AreEqual(null, record4);
        }

        [TestMethod]
        public void TestNext()
        {
            var dosKey = new Regulus.Utility.Doskey(10);

            dosKey.Record("in1");
            dosKey.Record("in2");
            dosKey.Record("in3");
            string record1 = dosKey.TryGetPrev();
            string record2 = dosKey.TryGetPrev();
            string record3 = dosKey.TryGetPrev();
            string record4 = dosKey.TryGetPrev();

            string record5 = dosKey.TryGetNext();
            string record6 = dosKey.TryGetNext();
            string record7 = dosKey.TryGetNext();
            string record8 = dosKey.TryGetNext();
            

            Assert.AreEqual("in3", record1);
            Assert.AreEqual("in2", record2);
            Assert.AreEqual("in1", record3);
            Assert.AreEqual(null, record4);
            Assert.AreEqual("in2", record5);
            Assert.AreEqual("in3", record6);
            Assert.AreEqual(null, record7);
            Assert.AreEqual(null, record8);
        }

        [TestMethod]
        public void TestNextPrev()
        {

            var dosKey = new Regulus.Utility.Doskey(3);


            dosKey.Record("in1");
            dosKey.Record("in2");
            dosKey.Record("in3");
            dosKey.Record("in4");
            dosKey.Record("in5");
            dosKey.Record("in6");
            string record1 = dosKey.TryGetNext();
            string record2 = dosKey.TryGetPrev();
            string record3 = dosKey.TryGetNext();
            string record4 = dosKey.TryGetPrev();

            string record5 = dosKey.TryGetNext();
            string record6 = dosKey.TryGetPrev();
            string record7 = dosKey.TryGetPrev();
            string record8 = dosKey.TryGetPrev();


            Assert.AreEqual(null, record1);
            Assert.AreEqual("in6", record2);
            Assert.AreEqual(null, record3);
            Assert.AreEqual("in5", record4);
            Assert.AreEqual("in6", record5);
            Assert.AreEqual("in5", record6);
            Assert.AreEqual("in4", record7);
            Assert.AreEqual(null, record8);
        }

        [TestMethod]
        public void TestPrevLimit()
        {
            var dosKey = new Regulus.Utility.Doskey(2);

            dosKey.Record("in1");
            dosKey.Record("in2");
            dosKey.Record("in3");
            string record1 = dosKey.TryGetPrev();
            string record2 = dosKey.TryGetPrev();
            string record3 = dosKey.TryGetPrev();
            string record4 = dosKey.TryGetPrev();

            Assert.AreEqual("in3", record1);
            Assert.AreEqual("in2", record2);
            Assert.AreEqual(null, record3);
            Assert.AreEqual(null, record4);
        }
    }
}
