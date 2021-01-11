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
            Xunit.Assert.Equal("ENUM1", desc1);

            TEST t2 = TEST.ENUM2;
            string desc2 = t2.GetEnumDescription();
            Xunit.Assert.Equal("ENUM2", desc2);
        }

        [Xunit.Fact]
        public void TestAllFlagEnumDescription()
        {
            TESTFLAG flags = TESTFLAG.ALL;

            List<string> descs = new List<string>();
            foreach (TESTFLAG flag in flags.GetFlags())
            {
                descs.Add(flag.GetEnumDescription());
            }

            Xunit.Assert.Equal("ENUM1", descs[0]);
            Xunit.Assert.Equal("ENUM2", descs[1]);
            Xunit.Assert.Equal("ENUM3", descs[2]);
            Xunit.Assert.Equal("ALL", descs[3]);
        }

        [Xunit.Fact]
        public void TestFlagEnumDescription()
        {
            TESTFLAG flags = TESTFLAG.ENUM1 | TESTFLAG.ENUM3;

            List<string> descs = new List<string>();
            foreach (TESTFLAG flag in flags.GetFlags())
            {
                descs.Add(flag.GetEnumDescription());
            }

            Xunit.Assert.Equal("ENUM1", descs[0]);
            Xunit.Assert.Equal("ENUM3", descs[1]);
        }

        [Xunit.Fact]
        public void TestForeachEnum1()
        {
            List<TEST> flags = new List<TEST>();
            foreach (TEST f in EnumHelper.GetEnums<TEST>())
            {
                flags.Add(f);
            }

            Xunit.Assert.Equal(TEST.ENUM1, flags[0]);
            Xunit.Assert.Equal(TEST.ENUM2, flags[1]);
            Xunit.Assert.Equal(TEST.ENUM3, flags[2]);
        }

        [Xunit.Fact]
        public void TestEnumFlags()
        {
            List<TESTFLAG> flags = new List<TESTFLAG>();

            foreach (TESTFLAG f in EnumHelper.GetEnums<TESTFLAG>())
            {
                flags.Add(f);
            }

            Xunit.Assert.Equal(TESTFLAG.ENUM1, flags[0]);
            Xunit.Assert.Equal(TESTFLAG.ENUM2, flags[1]);
            Xunit.Assert.Equal(TESTFLAG.ENUM3, flags[2]);
            Xunit.Assert.Equal(TESTFLAG.ALL, flags[3]);
        }

        [Xunit.Fact]
        public void TestCompareEnumFlags()
        {
            TESTFLAG flag = TESTFLAG.ENUM2 | TESTFLAG.ENUM3;
            TESTFLAG flagAll = TESTFLAG.ALL;
            List<TESTFLAG> flags = new List<TESTFLAG>();

            foreach (TESTFLAG f in EnumHelper.GetEnums<TESTFLAG>())
            {
                flags.Add(f);
            }

            Xunit.Assert.Equal(false, flag.HasFlag(flags[0]));
            Xunit.Assert.Equal(true, flag.HasFlag(flags[1]));
            Xunit.Assert.Equal(true, flag.HasFlag(flags[2]));
            Xunit.Assert.Equal(true, flagAll.HasFlag(flags[3]));
        }

        [Xunit.Fact]
        public void TestCompareEnumToBool()
        {
            TESTFLAG flag = TESTFLAG.ENUM2 | TESTFLAG.ENUM3;
            bool[] flags = flag.ToFlags().ToArray();
            Xunit.Assert.Equal(false, flags[0]);
            Xunit.Assert.Equal(true, flags[1]);
            Xunit.Assert.Equal(true, flags[2]);
        }
    }
}
