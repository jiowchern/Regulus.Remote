using System;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Data;

namespace GameTest
{
	[TestClass]
	public class DataTest
	{
		[TestMethod]
		public void TestAccountValid()
		{
			var account = new Account();

			Assert.AreNotEqual(null, account.Competnces);
			Assert.AreNotEqual(null, account.Name);
			Assert.AreNotEqual(null, account.Id);
			Assert.AreNotEqual(null, account.Password);
		}

		[TestMethod]
		public void TestValidRecord()
		{
			var record = new GamePlayerRecord();

			Assert.AreNotEqual(null, record.Id);
			Assert.AreEqual(0, record.Money);
			Assert.AreEqual(Guid.Empty, record.Owner);
		}

		[TestMethod]
		public void TestStageLock()
		{
			var stageLock = new StageLock();

			Assert.AreEqual(0, stageLock.KillCount);
			Assert.AreNotEqual(null, stageLock.Requires);
			Assert.AreEqual(0, stageLock.Stage);
		}
	}
}
