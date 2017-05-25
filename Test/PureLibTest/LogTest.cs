using System.Collections.Generic;





using Regulus.Utility;

namespace RegulusLibraryTest
{
	
	public class LogTest
	{
		[NUnit.Framework.Test()]
		public void TestWrite()
		{
			var messages = new List<string>();

			Singleton<Log>.Instance.RecordEvent += message =>
			{
				lock(messages)
					messages.Add(message);
			};

			Singleton<Log>.Instance.WriteInfo("123");
			Singleton<Log>.Instance.WriteInfo("456");
			Singleton<Log>.Instance.WriteInfo("789");
			Singleton<Log>.Instance.WriteInfo("123");
			Singleton<Log>.Instance.WriteInfo("1");

			while(true)
			{
				lock(messages)
				{
					if(messages.Count >= 5)
					{
						break;
					}
				}
			}

			NUnit.Framework.Assert.AreEqual("[Info]123", messages[0]);
			NUnit.Framework.Assert.AreEqual("[Info]456", messages[1]);
			NUnit.Framework.Assert.AreEqual("[Info]789", messages[2]);
			NUnit.Framework.Assert.AreEqual("[Info]123", messages[3]);
			NUnit.Framework.Assert.AreEqual("[Info]1", messages[4]);
		}
	}
}
