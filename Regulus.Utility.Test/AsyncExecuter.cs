using NUnit.Framework;

using Regulus.Collection;

namespace RegulusLibraryTest
{

    public class AsyncExecuter
    {

        [Test]
        public void TestAsyncExecuter()
        {



            Queue<int> ints = new Queue<int>();




            Regulus.Utility.AsyncExecuter threadQueue = new Regulus.Utility.AsyncExecuter();


            for (int i = 0; i < 10; ++i)
            {
                threadQueue.Push(new EnqueueHelper(ints, i).Run);
            }



            threadQueue.Shutdown();

            int[] values = ints.DequeueAll();
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

        }
    }
}
