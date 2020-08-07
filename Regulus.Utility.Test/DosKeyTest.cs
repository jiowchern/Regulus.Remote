


using Regulus.Utility;

namespace RegulusLibraryTest
{

    public class DosKeyTest
    {
        [NUnit.Framework.Test()]
        public void TestPrevInit()
        {
            Doskey dosKey = new Doskey(10);

            string record1 = dosKey.TryGetPrev();

            NUnit.Framework.Assert.AreEqual(null, record1);
        }

        [NUnit.Framework.Test()]
        public void TestPrev()
        {
            Doskey dosKey = new Doskey(10);

            dosKey.Record("in1");
            dosKey.Record("in2");
            dosKey.Record("in3");
            string record1 = dosKey.TryGetPrev();
            string record2 = dosKey.TryGetPrev();
            string record3 = dosKey.TryGetPrev();
            string record4 = dosKey.TryGetPrev();

            NUnit.Framework.Assert.AreEqual("in3", record1);
            NUnit.Framework.Assert.AreEqual("in2", record2);
            NUnit.Framework.Assert.AreEqual("in1", record3);
            NUnit.Framework.Assert.AreEqual(null, record4);
        }

        [NUnit.Framework.Test()]
        public void TestNext()
        {
            Doskey dosKey = new Doskey(10);

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

            NUnit.Framework.Assert.AreEqual("in3", record1);
            NUnit.Framework.Assert.AreEqual("in2", record2);
            NUnit.Framework.Assert.AreEqual("in1", record3);
            NUnit.Framework.Assert.AreEqual(null, record4);
            NUnit.Framework.Assert.AreEqual("in2", record5);
            NUnit.Framework.Assert.AreEqual("in3", record6);
            NUnit.Framework.Assert.AreEqual(null, record7);
            NUnit.Framework.Assert.AreEqual(null, record8);
        }

        [NUnit.Framework.Test()]
        public void TestNextPrev()
        {
            Doskey dosKey = new Doskey(3);

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

            NUnit.Framework.Assert.AreEqual(null, record1);
            NUnit.Framework.Assert.AreEqual("in6", record2);
            NUnit.Framework.Assert.AreEqual(null, record3);
            NUnit.Framework.Assert.AreEqual("in5", record4);
            NUnit.Framework.Assert.AreEqual("in6", record5);
            NUnit.Framework.Assert.AreEqual("in5", record6);
            NUnit.Framework.Assert.AreEqual("in4", record7);
            NUnit.Framework.Assert.AreEqual(null, record8);
        }

        [NUnit.Framework.Test()]
        public void TestPrevLimit()
        {
            Doskey dosKey = new Doskey(2);

            dosKey.Record("in1");
            dosKey.Record("in2");
            dosKey.Record("in3");
            string record1 = dosKey.TryGetPrev();
            string record2 = dosKey.TryGetPrev();
            string record3 = dosKey.TryGetPrev();
            string record4 = dosKey.TryGetPrev();

            NUnit.Framework.Assert.AreEqual("in3", record1);
            NUnit.Framework.Assert.AreEqual("in2", record2);
            NUnit.Framework.Assert.AreEqual(null, record3);
            NUnit.Framework.Assert.AreEqual(null, record4);
        }
    }
}
