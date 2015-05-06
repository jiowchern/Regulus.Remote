using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Extension;
namespace RemotingTest
{
    [TestClass]
    public class EnumFlag
    {
        [Flags]
        enum TESTFLAG
        {
            F1 = 1,
            F2 = 2,
            F3 = 4,
            F4 = 8
        }
        [TestMethod]
        public void TestEnumFlagForeach()
        {
            TESTFLAG flags = TESTFLAG.F2 | TESTFLAG.F3;
            TESTFLAG[] result = new TESTFLAG[2];
            int i = 0;
            foreach (TESTFLAG flag in flags.GetFlags())
            {
                result[i++] = flag;
            }
            Assert.AreEqual(TESTFLAG.F2, result[0]);
            Assert.AreEqual(TESTFLAG.F3, result[1]);
        }
    }
}
