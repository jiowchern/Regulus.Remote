


using Regulus.Utility;

namespace RegulusLibraryTest
{
	
	public class DosKeyTest
	{
		[NUnit.Framework.Test()]
		public void TestPrevInit()
		{
			var dosKey = new Doskey(10);

			var record1 = dosKey.TryGetPrev();

			NUnit.Framework.Assert.AreEqual(null, record1);
		}

		[NUnit.Framework.Test()]
		public void TestPrev()
		{
			var dosKey = new Doskey(10);

			dosKey.Record("in1");
			dosKey.Record("in2");
			dosKey.Record("in3");
			var record1 = dosKey.TryGetPrev();
			var record2 = dosKey.TryGetPrev();
			var record3 = dosKey.TryGetPrev();
			var record4 = dosKey.TryGetPrev();

			NUnit.Framework.Assert.AreEqual("in3", record1);
			NUnit.Framework.Assert.AreEqual("in2", record2);
			NUnit.Framework.Assert.AreEqual("in1", record3);
			NUnit.Framework.Assert.AreEqual(null, record4);
		}

		[NUnit.Framework.Test()]
		public void TestNext()
		{
			var dosKey = new Doskey(10);

			dosKey.Record("in1");
			dosKey.Record("in2");
			dosKey.Record("in3");
			var record1 = dosKey.TryGetPrev();
			var record2 = dosKey.TryGetPrev();
			var record3 = dosKey.TryGetPrev();
			var record4 = dosKey.TryGetPrev();

			var record5 = dosKey.TryGetNext();
			var record6 = dosKey.TryGetNext();
			var record7 = dosKey.TryGetNext();
			var record8 = dosKey.TryGetNext();

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
			var dosKey = new Doskey(3);

			dosKey.Record("in1");
			dosKey.Record("in2");
			dosKey.Record("in3");
			dosKey.Record("in4");
			dosKey.Record("in5");
			dosKey.Record("in6");
			var record1 = dosKey.TryGetNext();
			var record2 = dosKey.TryGetPrev();
			var record3 = dosKey.TryGetNext();
			var record4 = dosKey.TryGetPrev();

			var record5 = dosKey.TryGetNext();
			var record6 = dosKey.TryGetPrev();
			var record7 = dosKey.TryGetPrev();
			var record8 = dosKey.TryGetPrev();

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
			var dosKey = new Doskey(2);

			dosKey.Record("in1");
			dosKey.Record("in2");
			dosKey.Record("in3");
			var record1 = dosKey.TryGetPrev();
			var record2 = dosKey.TryGetPrev();
			var record3 = dosKey.TryGetPrev();
			var record4 = dosKey.TryGetPrev();

			NUnit.Framework.Assert.AreEqual("in3", record1);
			NUnit.Framework.Assert.AreEqual("in2", record2);
			NUnit.Framework.Assert.AreEqual(null, record3);
			NUnit.Framework.Assert.AreEqual(null, record4);
		}
	}
}
