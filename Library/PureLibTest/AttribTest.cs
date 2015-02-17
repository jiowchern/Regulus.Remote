using System;
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
    }
}
