// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlagTest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the TESTENUM type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus;
using Regulus.CustomType;

#endregion

namespace PureLibraryTest
{
	internal enum TESTENUM
	{
		_1, 

		_2, 

		_3
	};

	[TestClass]
	public class FlagTest
	{
		[TestMethod]
		public void TestToArray()
		{
			var flags = new Flag<TESTENUM>();
			flags[TESTENUM._1] = true;
			flags[TESTENUM._2] = false;
			flags[TESTENUM._3] = true;

			var array = flags.ToArray();

			Assert.AreNotEqual(TESTENUM._2, array[0]);
			Assert.AreNotEqual(TESTENUM._2, array[1]);
		}

		[TestMethod]
		public void TestCustomFlag1()
		{
			var flags = new Flag<TESTENUM>();
			flags[TESTENUM._1] = false;
			flags[TESTENUM._2] = true;
			flags[TESTENUM._3] = false;

			Assert.AreEqual(true, flags[TESTENUM._2]);
			Assert.AreEqual(false, flags[TESTENUM._3]);
		}

		[TestMethod]
		public void TestCustomFlag2()
		{
			var flags = new Flag<TESTENUM>(TESTENUM._1, TESTENUM._3);

			Assert.AreEqual(false, flags[TESTENUM._2]);
			Assert.AreEqual(true, flags[TESTENUM._3]);
		}

		[TestMethod]
		public void TestCustomFlagSerializer()
		{
			var flags = new Flag<TESTENUM>(TESTENUM._1, TESTENUM._3);

			var buffer = TypeHelper.Serializer(flags);
			var flags2 = TypeHelper.Deserialize<Flag<TESTENUM>>(buffer);

			Assert.AreEqual(false, flags2[TESTENUM._2]);
			Assert.AreEqual(true, flags2[TESTENUM._3]);
		}

		[TestMethod]
		public void TestCustomFlagConvert()
		{
			var flags = new Flag<TESTENUM>(TESTENUM._1, TESTENUM._3);
			var objs = (from e in flags select (object)e).ToArray();

			Flag<TESTENUM> flag2 = objs;

			Assert.AreEqual(false, flag2[TESTENUM._2]);
			Assert.AreEqual(true, flag2[TESTENUM._3]);
		}
	}
}