// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemotingValueResultTest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the RemotingValueResultTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Timers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.Remoting;

#endregion

namespace PureLibraryTest
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