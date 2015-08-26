using System;
using System.Collections.Generic;
using System.Linq;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.Collection;

namespace RegulusLibraryTest
{
    [TestClass]
    public class UnitTestDifferenceNoticer
    {
        [TestMethod]
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
            Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[0] , new[] {1,2,3,4,5}));
            Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(vertifyJoin[1], new[] { 6 }));
            Assert.IsTrue(Regulus.Utility.ValueHelper.DeepEqual(vertifyLeft[1], new[] { 1 }));
        }
    }
}
