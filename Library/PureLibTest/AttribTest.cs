using System;
using System.Collections.Generic;
using System.Linq;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.Utility;

namespace RegulusLibraryTest
{
	/// <summary>
	///     AttribTest 的摘要描述
	/// </summary>
	[TestClass]
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

		[TestMethod]
		public void TestEnumDescription()
		{
			var t1 = TEST.ENUM1;
			var desc1 = t1.GetEnumDescription();
			Assert.AreEqual("ENUM1", desc1);

			var t2 = TEST.ENUM2;
			var desc2 = t2.GetEnumDescription();
			Assert.AreEqual("ENUM2", desc2);
		}

		[TestMethod]
		public void TestAllFlagEnumDescription()
		{
			var flags = TESTFLAG.ALL;

			var descs = new List<string>();
			foreach(TESTFLAG flag in flags.GetFlags())
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
			var flags = TESTFLAG.ENUM1 | TESTFLAG.ENUM3;

			var descs = new List<string>();
			foreach(TESTFLAG flag in flags.GetFlags())
			{
				descs.Add(flag.GetEnumDescription());
			}

			Assert.AreEqual("ENUM1", descs[0]);
			Assert.AreEqual("ENUM3", descs[1]);
		}

		[TestMethod]
		public void TestForeachEnum1()
		{
			var flags = new List<TEST>();
			foreach(var f in EnumHelper.GetEnums<TEST>())
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
			var flags = new List<TESTFLAG>();

			foreach(var f in EnumHelper.GetEnums<TESTFLAG>())
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
			var flag = TESTFLAG.ENUM2 | TESTFLAG.ENUM3;
			var flagAll = TESTFLAG.ALL;
			var flags = new List<TESTFLAG>();

			foreach(var f in EnumHelper.GetEnums<TESTFLAG>())
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
			var flag = TESTFLAG.ENUM2 | TESTFLAG.ENUM3;
			var flags = flag.ToFlags().ToArray();
			Assert.AreEqual(false, flags[0]);
			Assert.AreEqual(true, flags[1]);
			Assert.AreEqual(true, flags[2]);
		}
	}
}
