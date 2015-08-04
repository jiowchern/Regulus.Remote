// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogTest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the LogTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.Utility;

#endregion

namespace PureLibraryTest
{
	[TestClass]
	public class LogTest
	{
		[TestMethod]
		public void TestWrite()
		{
			var messages = new List<string>();

			Singleton<Log>.Instance.RecordEvent += message =>
			{
				lock (messages)
					messages.Add(message);
			};

			Singleton<Log>.Instance.WriteInfo("123");
			Singleton<Log>.Instance.WriteInfo("456");
			Singleton<Log>.Instance.WriteInfo("789");
			Singleton<Log>.Instance.WriteInfo("123");
			Singleton<Log>.Instance.WriteInfo("1");

			while (true)
			{
				lock (messages)
				{
					if (messages.Count >= 5)
					{
						break;
					}
				}
			}

			Assert.AreEqual("[Info]123", messages[0]);
			Assert.AreEqual("[Info]456", messages[1]);
			Assert.AreEqual("[Info]789", messages[2]);
			Assert.AreEqual("[Info]123", messages[3]);
			Assert.AreEqual("[Info]1", messages[4]);
		}
	}
}