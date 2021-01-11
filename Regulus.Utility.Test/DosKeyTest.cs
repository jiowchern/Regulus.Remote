


using Regulus.Utility;

namespace RegulusLibraryTest
{

    public class DosKeyTest
    {
        [Xunit.Fact]
        public void TestPrevInit()
        {
            Doskey dosKey = new Doskey(10);

            string record1 = dosKey.TryGetPrev();

            Xunit.Assert.Null(record1);
        }

        [Xunit.Fact]
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

            Xunit.Assert.Equal("in3", record1);
            Xunit.Assert.Equal("in2", record2);
            Xunit.Assert.Equal("in1", record3);
            Xunit.Assert.Equal(null, record4);
        }

        [Xunit.Fact]
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

            Xunit.Assert.Equal("in3", record1);
            Xunit.Assert.Equal("in2", record2);
            Xunit.Assert.Equal("in1", record3);
            Xunit.Assert.Equal(null, record4);
            Xunit.Assert.Equal("in2", record5);
            Xunit.Assert.Equal("in3", record6);
            Xunit.Assert.Equal(null, record7);
            Xunit.Assert.Equal(null, record8);
        }

        [Xunit.Fact]
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

            Xunit.Assert.Equal(null, record1);
            Xunit.Assert.Equal("in6", record2);
            Xunit.Assert.Equal(null, record3);
            Xunit.Assert.Equal("in5", record4);
            Xunit.Assert.Equal("in6", record5);
            Xunit.Assert.Equal("in5", record6);
            Xunit.Assert.Equal("in4", record7);
            Xunit.Assert.Equal(null, record8);
        }

        [Xunit.Fact]
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

            Xunit.Assert.Equal("in3", record1);
            Xunit.Assert.Equal("in2", record2);
            Xunit.Assert.Equal(null, record3);
            Xunit.Assert.Equal(null, record4);
        }
    }
}
