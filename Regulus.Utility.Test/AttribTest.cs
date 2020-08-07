using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RegulusLibraryTest
{
    /// <summary>
    ///     AttribTest 的摘要描述
    /// </summary>

    public class AttribTest
    {
        public enum TEST
        {
            [EnumDescription("ENUM1")]
            ENUM1,

            [EnumDescription("ENUM2")]
            ENUM2,

            [EnumDescription("ENUM3")]
            ENUM3
        };

        [Flags]
        public enum TESTFLAG
        {
            [EnumDescription("ENUM1")]
            ENUM1 = 1,

            [EnumDescription("ENUM2")]
            ENUM2 = 2,

            [EnumDescription("ENUM3")]
            ENUM3 = 4,

            [EnumDescription("ALL")]
            ALL = int.MaxValue
        };

        public void TestEnumDescription()
        {
            TEST t1 = TEST.ENUM1;
            string desc1 = t1.GetEnumDescription();
            NUnit.Framework.Assert.AreEqual("ENUM1", desc1);

            TEST t2 = TEST.ENUM2;
            string desc2 = t2.GetEnumDescription();
            NUnit.Framework.Assert.AreEqual("ENUM2", desc2);
        }

        [NUnit.Framework.Test()]
        public void TestAllFlagEnumDescription()
        {
            TESTFLAG flags = TESTFLAG.ALL;

            List<string> descs = new List<string>();
            foreach (TESTFLAG flag in flags.GetFlags())
            {
                descs.Add(flag.GetEnumDescription());
            }

            NUnit.Framework.Assert.AreEqual("ENUM1", descs[0]);
            NUnit.Framework.Assert.AreEqual("ENUM2", descs[1]);
            NUnit.Framework.Assert.AreEqual("ENUM3", descs[2]);
            NUnit.Framework.Assert.AreEqual("ALL", descs[3]);
        }

        [NUnit.Framework.Test()]
        public void TestFlagEnumDescription()
        {
            TESTFLAG flags = TESTFLAG.ENUM1 | TESTFLAG.ENUM3;

            List<string> descs = new List<string>();
            foreach (TESTFLAG flag in flags.GetFlags())
            {
                descs.Add(flag.GetEnumDescription());
            }

            NUnit.Framework.Assert.AreEqual("ENUM1", descs[0]);
            NUnit.Framework.Assert.AreEqual("ENUM3", descs[1]);
        }

        [NUnit.Framework.Test()]
        public void TestForeachEnum1()
        {
            List<TEST> flags = new List<TEST>();
            foreach (TEST f in EnumHelper.GetEnums<TEST>())
            {
                flags.Add(f);
            }

            NUnit.Framework.Assert.AreEqual(TEST.ENUM1, flags[0]);
            NUnit.Framework.Assert.AreEqual(TEST.ENUM2, flags[1]);
            NUnit.Framework.Assert.AreEqual(TEST.ENUM3, flags[2]);
        }

        [NUnit.Framework.Test()]
        public void TestEnumFlags()
        {
            List<TESTFLAG> flags = new List<TESTFLAG>();

            foreach (TESTFLAG f in EnumHelper.GetEnums<TESTFLAG>())
            {
                flags.Add(f);
            }

            NUnit.Framework.Assert.AreEqual(TESTFLAG.ENUM1, flags[0]);
            NUnit.Framework.Assert.AreEqual(TESTFLAG.ENUM2, flags[1]);
            NUnit.Framework.Assert.AreEqual(TESTFLAG.ENUM3, flags[2]);
            NUnit.Framework.Assert.AreEqual(TESTFLAG.ALL, flags[3]);
        }

        [NUnit.Framework.Test()]
        public void TestCompareEnumFlags()
        {
            TESTFLAG flag = TESTFLAG.ENUM2 | TESTFLAG.ENUM3;
            TESTFLAG flagAll = TESTFLAG.ALL;
            List<TESTFLAG> flags = new List<TESTFLAG>();

            foreach (TESTFLAG f in EnumHelper.GetEnums<TESTFLAG>())
            {
                flags.Add(f);
            }

            NUnit.Framework.Assert.AreEqual(false, flag.HasFlag(flags[0]));
            NUnit.Framework.Assert.AreEqual(true, flag.HasFlag(flags[1]));
            NUnit.Framework.Assert.AreEqual(true, flag.HasFlag(flags[2]));
            NUnit.Framework.Assert.AreEqual(true, flagAll.HasFlag(flags[3]));
        }

        [NUnit.Framework.Test()]
        public void TestCompareEnumToBool()
        {
            TESTFLAG flag = TESTFLAG.ENUM2 | TESTFLAG.ENUM3;
            bool[] flags = flag.ToFlags().ToArray();
            NUnit.Framework.Assert.AreEqual(false, flags[0]);
            NUnit.Framework.Assert.AreEqual(true, flags[1]);
            NUnit.Framework.Assert.AreEqual(true, flags[2]);
        }
    }
}
