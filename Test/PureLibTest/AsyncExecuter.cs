using System;

using NUnit.Framework;

using Regulus.Collection;

namespace RegulusLibraryTest
{
	
	public class AsyncExecuter
	{
		[Test]
		public void TestAsyncExecuter()
		{
			var ints = new Queue<int>();
			var actions = new Queue<Action>();

			for(var i = 0; i < 10; ++i)
			{
				actions.Enqueue(new EnqueueHelper(ints, i).Run);
			}

			var threadQueue = new Regulus.Utility.AsyncExecuter(actions.DequeueAll());

			threadQueue.WaitDone();

			var values = ints.DequeueAll();
			NUnit.Framework.Assert.AreEqual(0, values[0]);
			NUnit.Framework.Assert.AreEqual(1, values[1]);
			NUnit.Framework.Assert.AreEqual(2, values[2]);
			NUnit.Framework.Assert.AreEqual(3, values[3]);
			NUnit.Framework.Assert.AreEqual(4, values[4]);
			NUnit.Framework.Assert.AreEqual(5, values[5]);
			NUnit.Framework.Assert.AreEqual(6, values[6]);
			NUnit.Framework.Assert.AreEqual(7, values[7]);
			NUnit.Framework.Assert.AreEqual(8, values[8]);
			NUnit.Framework.Assert.AreEqual(9, values[9]);

			threadQueue.Push(new EnqueueHelper(ints, 10).Run);
			threadQueue.Push(new EnqueueHelper(ints, 11).Run);

			threadQueue.Push(new EnqueueHelper(ints, 12).Run);

			threadQueue.WaitDone();
			var values2 = ints.DequeueAll();
			NUnit.Framework.Assert.AreEqual(10, values2[0]);
			NUnit.Framework.Assert.AreEqual(11, values2[1]);
			NUnit.Framework.Assert.AreEqual(12, values2[2]);
		}
	}
}
