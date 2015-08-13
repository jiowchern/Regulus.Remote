using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.Net45;
using Regulus.Remoting;

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
