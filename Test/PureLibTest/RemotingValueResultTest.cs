using System.Timers;

using NUnit.Framework;

using Regulus.Remoting;

namespace RegulusLibraryTest
{
	
	public class RemotingValueResultTest
	{
		[NUnit.Framework.Test()]
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
