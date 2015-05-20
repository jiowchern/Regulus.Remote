using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonTest
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void TestAccountValid()
        {
            VGame.Project.FishHunter.Data.Account account = new VGame.Project.FishHunter.Data.Account();

            Assert.AreNotEqual(null, account.Competnces);
            Assert.AreNotEqual(null, account.Name);
            Assert.AreNotEqual(null, account.Id);
            Assert.AreNotEqual(null, account.Password);            
        }

        [TestMethod]
        public void TestValidRecord()
        {
            VGame.Project.FishHunter.Data.Record record = new VGame.Project.FishHunter.Data.Record();

            Assert.AreNotEqual(null, record.Id);
            Assert.AreEqual(0, record.Money);
            Assert.AreEqual(Guid.Empty, record.Owner);
        }
    }
}
