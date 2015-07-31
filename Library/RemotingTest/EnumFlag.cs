// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumFlag.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the EnumFlag type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.Utility;

#endregion

namespace RemotingTest
{
	[TestClass]
	public class EnumFlag
	{
		[Flags]
		private enum TESTFLAG
		{
			F1 = 1, 

			F2 = 2, 

			F3 = 4, 

			F4 = 8
		}

		[TestMethod]
		public void TestEnumFlagForeach()
		{
			var flags = TESTFLAG.F2 | TESTFLAG.F3;
			var result = new TESTFLAG[2];
			var i = 0;
			foreach (TESTFLAG flag in flags.GetFlags())
			{
				result[i++] = flag;
			}

			Assert.AreEqual(TESTFLAG.F2, result[0]);
			Assert.AreEqual(TESTFLAG.F3, result[1]);
		}
	}
}