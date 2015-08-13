using System.Timers;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using Regulus.Remoting;

namespace RegulusLibraryTest
{
	[TestClass]
	public class RemotingValueResultTest
	{
		[TestMethod]
		[Timeout(5000)]
		public void TestRemotingValueResult()
		{
			var val = new Value<bool>();
			var timer = new Timer(1);
			timer.Start();
			timer.Elapsed += (object sender, ElapsedEventArgs e) => { val.SetValue(true); };

			val.Result();
		}
	}
}
