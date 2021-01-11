using Regulus.Collection;
using System.Collections.Generic;
using System.Linq;

namespace RegulusLibraryTest
{

    public class UnitTestDifferenceNoticer
    {
        [Xunit.Fact]
        public void DifferenceNoticerTestSet()
        {
            List<int[]> joinResult = new List<int[]>();
            List<int[]> leftResult = new List<int[]>();
            int[] before = new int[]
            {
                1,
                2,
                3,
                4,
                5
            };

            int[] after = new int[]
            {
                2,3,4,5,6
            };
            DifferenceNoticer<int> differenceNoticer = new DifferenceNoticer<int>();
            differenceNoticer.JoinEvent += instances => { joinResult.Add(instances.ToArray()); };
            differenceNoticer.LeaveEvent += instances => { leftResult.Add(instances.ToArray()); };

            differenceNoticer.Set(before); // j0 1 , 2 ,3 ,4 ,5 : l0
            differenceNoticer.Set(after); // j1 6 : l1 1

            int[][] vertifyJoin = joinResult.ToArray();
            int[][] vertifyLeft = leftResult.ToArray();
            Xunit.Assert.True(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[0], new[] { 1, 2, 3, 4, 5 }));
            Xunit.Assert.True(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[1], new[] { 6 }));
            Xunit.Assert.True(Regulus.Utility.ValueHelper.DeepEqual(vertifyLeft[1], new[] { 1 }));
        }

        [Xunit.Fact]
        public void DifferenceNoticerTestSetWithParam()
        {
            List<int[]> joinResult = new List<int[]>();
            List<int[]> leftResult = new List<int[]>();
            int[] before = new int[]
            {
                1,
                2,
                3,
                4,
                5
            };

            int[] after = new int[]
            {
                2,3,4,5,6
            };
            DifferenceNoticer<int> differenceNoticer = new DifferenceNoticer<int>(new IntComparer());
            differenceNoticer.JoinEvent += instances => { joinResult.Add(instances.ToArray()); };
            differenceNoticer.LeaveEvent += instances => { leftResult.Add(instances.ToArray()); };

            differenceNoticer.Set(before); // j0 1 , 2 ,3 ,4 ,5 : l0
            differenceNoticer.Set(after); // j1 6 : l1 1

            int[][] vertifyJoin = joinResult.ToArray();
            int[][] vertifyLeft = leftResult.ToArray();
            Xunit.Assert.True(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[0], new[] { 1, 2, 3, 4, 5 }));
            Xunit.Assert.True(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[1], new[] { 6 }));
            Xunit.Assert.True(Regulus.Utility.ValueHelper.DeepEqual(vertifyLeft[1], new[] { 1 }));
        }
    }

    public class IntComparer : IEqualityComparer<int>
    {
        bool IEqualityComparer<int>.Equals(int x, int y)
        {
            return x == y;
        }

        int IEqualityComparer<int>.GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
    }
}
