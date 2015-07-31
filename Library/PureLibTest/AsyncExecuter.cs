// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncExecuter.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AsyncExecuter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.Collection;

#endregion

namespace PureLibraryTest
{
	[TestClass]
	public class AsyncExecuter
	{
		[TestMethod]
		public void TestAsyncExecuter()
		{
			var ints = new Queue<int>();
			var actions = new Queue<Action>();

			for (var i = 0; i < 10; ++i)
			{
				actions.Enqueue(new EnqueueHelper(ints, i).Run);
			}

			var threadQueue = new Regulus.Utility.AsyncExecuter(actions.DequeueAll());

			threadQueue.WaitDone();

			var values = ints.DequeueAll();
			Assert.AreEqual(0, values[0]);
			Assert.AreEqual(1, values[1]);
			Assert.AreEqual(2, values[2]);
			Assert.AreEqual(3, values[3]);
			Assert.AreEqual(4, values[4]);
			Assert.AreEqual(5, values[5]);
			Assert.AreEqual(6, values[6]);
			Assert.AreEqual(7, values[7]);
			Assert.AreEqual(8, values[8]);
			Assert.AreEqual(9, values[9]);

			threadQueue.Push(new EnqueueHelper(ints, 10).Run);
			threadQueue.Push(new EnqueueHelper(ints, 11).Run);

			threadQueue.Push(new EnqueueHelper(ints, 12).Run);

			threadQueue.WaitDone();
			var values2 = ints.DequeueAll();
			Assert.AreEqual(10, values2[0]);
			Assert.AreEqual(11, values2[1]);
			Assert.AreEqual(12, values2[2]);
		}
	}
}