using System;
using System.Collections.Generic;
using System.Linq;





using Regulus.Collection;

namespace RegulusLibraryTest
{
    
    public class UnitTestDifferenceNoticer
    {
        [NUnit.Framework.Test()]
        public void DifferenceNoticerTestSet()
        {
            var joinResult = new List<int[]>();
            var leftResult = new List<int[]>();
            var before = new int[]
            {
                1,
                2,
                3,
                4,
                5
            };

            var after = new int[]
            {
                2,3,4,5,6
            };
            var differenceNoticer = new DifferenceNoticer<int>();
            differenceNoticer.JoinEvent += instances => { joinResult.Add(instances.ToArray()); };
            differenceNoticer.LeftEvent += instances => { leftResult.Add(instances.ToArray()); };

            differenceNoticer.Set(before); // j0 1 , 2 ,3 ,4 ,5 : l0
            differenceNoticer.Set(after); // j1 6 : l1 1

            var vertifyJoin = joinResult.ToArray();
            var vertifyLeft = leftResult.ToArray();
            NUnit.Framework.Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[0] , new[] {1,2,3,4,5}));
            NUnit.Framework.Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[1], new[] { 6 }));
            NUnit.Framework.Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(vertifyLeft[1], new[] { 1 }));
        }

        [NUnit.Framework.Test()]
        public void DifferenceNoticerTestSetWithParam()
        {
            var joinResult = new List<int[]>();
            var leftResult = new List<int[]>();
            var before = new int[]
            {
                1,
                2,
                3,
                4,
                5
            };

            var after = new int[]
            {
                2,3,4,5,6
            };
            var differenceNoticer = new DifferenceNoticer<int>( new IntComparer() );
            differenceNoticer.JoinEvent += instances => { joinResult.Add(instances.ToArray()); };
            differenceNoticer.LeftEvent += instances => { leftResult.Add(instances.ToArray()); };

            differenceNoticer.Set(before); // j0 1 , 2 ,3 ,4 ,5 : l0
            differenceNoticer.Set(after); // j1 6 : l1 1

            var vertifyJoin = joinResult.ToArray();
            var vertifyLeft = leftResult.ToArray();
            NUnit.Framework.Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[0], new[] { 1, 2, 3, 4, 5 }));
            NUnit.Framework.Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[1], new[] { 6 }));
            NUnit.Framework.Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(vertifyLeft[1], new[] { 1 }));
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
