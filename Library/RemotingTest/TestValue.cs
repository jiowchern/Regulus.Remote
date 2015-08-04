// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestValue.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ValueTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.Net45;
using Regulus.Remoting;

#endregion

namespace RemotingTest
{
	[TestClass]
	public class ValueTest
	{
		private async void _TestAwait()
		{
			var val = new Value<bool>();

			var task = val.ToTask();

			val.SetValue(true);
			var retValue = await task;
			Assert.AreEqual(true, retValue);
		}

		[TestMethod]
		public void TestAwait()
		{
			_TestAwait();
		}
	}
}