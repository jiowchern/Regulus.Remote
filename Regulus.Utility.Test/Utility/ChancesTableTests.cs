using NUnit.Framework;
using System.Collections.Generic;

namespace Regulus.Utility.Tests
{
    [TestFixture()]
    public class ChancesTableTests
    {
        

        [Test()]
        public void Valid()
        {
            IEnumerable<ChancesTable<int>.Item> items = new ChancesTable<int>.Item[] { new ChancesTable<int>.Item {  Target = 1 , Scale = 0  } , new ChancesTable<int>.Item { Target = 2, Scale =1 } , new ChancesTable<int>.Item { Target = 3, Scale = 1 } };  
            var table = new Regulus.Utility.ChancesTable<int>(items);
            var val1 = table.Get(0);
            var val2 = table.Get(1);
            var val3 = table.Get(2);
            
            Assert.AreEqual(1,val1);
            Assert.AreEqual(2, val2);
            Assert.AreEqual(3, val3);            
        }
    }
}