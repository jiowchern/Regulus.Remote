using Xunit;

using Regulus.Collection;

namespace RegulusLibraryTest
{

    public class AsyncExecuter
    {

        [Xunit.Fact]
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
            Xunit.Assert.Equal(0, values[0]);
            Xunit.Assert.Equal(1, values[1]);
            Xunit.Assert.Equal(2, values[2]);
            Xunit.Assert.Equal(3, values[3]);
            Xunit.Assert.Equal(4, values[4]);
            Xunit.Assert.Equal(5, values[5]);
            Xunit.Assert.Equal(6, values[6]);
            Xunit.Assert.Equal(7, values[7]);
            Xunit.Assert.Equal(8, values[8]);
            Xunit.Assert.Equal(9, values[9]);

        }
    }
}
