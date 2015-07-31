using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PureLibTest
{
    using Regulus.Extension;
    /// <summary>
    /// AttribTest 的摘要描述
    /// </summary>
    [TestClass]
    public class AttribTest
    {

        public enum TEST
        {
            [Regulus.Utility.EnumDescription("ENUM1")]
            ENUM1,
            [Regulus.Utility.EnumDescription("ENUM2")]
            ENUM2,
            [Regulus.Utility.EnumDescription("ENUM3")]
            ENUM3,
        };

        [Flags]
        public enum TESTFLAG
        {
            [Regulus.Utility.EnumDescription("ENUM1")]
            ENUM1 = 1,
            [Regulus.Utility.EnumDescription("ENUM2")]
            ENUM2 = 2,
            [Regulus.Utility.EnumDescription("ENUM3")]
            ENUM3 = 4,
            [Regulus.Utility.EnumDescription("ALL")]
            ALL = int.MaxValue
        };

        [TestMethod]
        public void TestEnumDescription()
        {
            TEST t1 = TEST.ENUM1;
            string desc1 = t1.GetEnumDescription();
            Assert.AreEqual("ENUM1", desc1);

            TEST t2 = TEST.ENUM2;
            string desc2 = t2.GetEnumDescription();
            Assert.AreEqual("ENUM2", desc2);


        }

        [TestMethod]
        public void TestAllFlagEnumDescription()
        {
            TESTFLAG flags = TESTFLAG.ALL;

            List<string> descs = new List<string>();
            foreach (TESTFLAG flag in flags.GetFlags())
            {
                descs.Add(flag.GetEnumDescription());
            }

            Assert.AreEqual("ENUM1", descs[0]);
            Assert.AreEqual("ENUM2", descs[1]);
            Assert.AreEqual("ENUM3", descs[2]);
            Assert.AreEqual("ALL", descs[3]);
        }


        [TestMethod]
        public void TestFlagEnumDescription()
        {
            TESTFLAG flags = TESTFLAG.ENUM1 | TESTFLAG.ENUM3;

            List<string> descs = new List<string>();
            foreach (TESTFLAG flag in flags.GetFlags())
            {
                descs.Add(flag.GetEnumDescription());
            }

            Assert.AreEqual("ENUM1", descs[0]);
            Assert.AreEqual("ENUM3", descs[1]);
        }
        

        [TestMethod]
        public void TestForeachEnum1()
        {
            List<TEST> flags = new List<TEST>();
            foreach (TEST f in EnumHelper.GetEnums<TEST>())                
            {
                flags.Add(f);
            }

            Assert.AreEqual(TEST.ENUM1, flags[0]);
            Assert.AreEqual(TEST.ENUM2, flags[1]);
            Assert.AreEqual(TEST.ENUM3, flags[2]);
        }
        [TestMethod]
        public void TestEnumFlags()
        {
            List<TESTFLAG> flags = new List<TESTFLAG>();

            foreach (TESTFLAG f in EnumHelper.GetEnums<TESTFLAG>())
            {
                flags.Add(f);
            }
            Assert.AreEqual(TESTFLAG.ENUM1, flags[0]);
            Assert.AreEqual(TESTFLAG.ENUM2, flags[1]);
            Assert.AreEqual(TESTFLAG.ENUM3, flags[2]);
            Assert.AreEqual(TESTFLAG.ALL, flags[3]);

        }

        [TestMethod]
        public void TestCompareEnumFlags()
        {
            TESTFLAG flag = TESTFLAG.ENUM2 | TESTFLAG.ENUM3;
            TESTFLAG flagAll = TESTFLAG.ALL;
            List<TESTFLAG> flags = new List<TESTFLAG>();

            foreach (TESTFLAG f in EnumHelper.GetEnums<TESTFLAG>())
            {
                flags.Add(f);
            }
            

            Assert.AreEqual(false, flag.HasFlag(flags[0]));
            Assert.AreEqual(true, flag.HasFlag(flags[1]));
            Assert.AreEqual(true, flag.HasFlag(flags[2]));
            Assert.AreEqual(true, flagAll.HasFlag(flags[3]));
            

        }        
        [TestMethod]
        public void TestCompareEnumToBool()
        {
            TESTFLAG flag = TESTFLAG.ENUM2 | TESTFLAG.ENUM3;
            var flags = flag.ToFlags().ToArray();
            Assert.AreEqual(false, flags[0]);
            Assert.AreEqual(true, flags[1]);
            Assert.AreEqual(true, flags[2]);
        }

        
    }
}
