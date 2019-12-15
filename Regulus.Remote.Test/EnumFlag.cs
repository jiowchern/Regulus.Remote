using System;





using Regulus.Utility;

namespace RemotingTest
{
	
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

		[NUnit.Framework.Test()]
		public void TestEnumFlagForeach()
		{
			var flags = TESTFLAG.F2 | TESTFLAG.F3;
			var result = new TESTFLAG[2];
			var i = 0;
			foreach(TESTFLAG flag in flags.GetFlags())
			{
				result[i++] = flag;
			}

			NUnit.Framework.Assert.AreEqual(TESTFLAG.F2, result[0]);
			NUnit.Framework.Assert.AreEqual(TESTFLAG.F3, result[1]);
		}
	}
}
