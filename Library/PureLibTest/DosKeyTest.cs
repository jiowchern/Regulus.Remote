// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DosKeyTest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the DosKeyTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.Utility;

#endregion

namespace PureLibraryTest
{
	[TestClass]
	public class DosKeyTest
	{
		[TestMethod]
		public void TestPrevInit()
		{
			var dosKey = new Doskey(10);

			var record1 = dosKey.TryGetPrev();

			Assert.AreEqual(null, record1);
		}

		[TestMethod]
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

			Assert.AreEqual("in3", record1);
			Assert.AreEqual("in2", record2);
			Assert.AreEqual("in1", record3);
			Assert.AreEqual(null, record4);
		}

		[TestMethod]
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
			var dosKey = new Doskey(2);

			dosKey.Record("in1");
			dosKey.Record("in2");
			dosKey.Record("in3");
			var record1 = dosKey.TryGetPrev();
			var record2 = dosKey.TryGetPrev();
			var record3 = dosKey.TryGetPrev();
			var record4 = dosKey.TryGetPrev();

			Assert.AreEqual("in3", record1);
			Assert.AreEqual("in2", record2);
			Assert.AreEqual(null, record3);
			Assert.AreEqual(null, record4);
		}
	}
}