using System;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace GameTest
{
    [Binding]
	[Scope(Feature = "WeaponChancesTable")]
    public class WeaponChancesTableSteps
    {
        VGame.Project.FishHunter.WeaponChancesTable _WeaponChancesTable;
        [Given(@"武器清單是")]
        public void Given武器清單是(Table table)
        {
            _WeaponChancesTable = new VGame.Project.FishHunter.WeaponChancesTable(_ToData(table));
        }

        private VGame.Project.FishHunter.WeaponChancesTable.Data[] _ToData(Table table)
        {
            var datas = table.CreateSet<VGame.Project.FishHunter.WeaponChancesTable.Data>();

            return datas.ToArray();
        }
        
        [When(@"機率是""(.*)""")]
        public void When機率是(Decimal p0)
        {
            
            var id = _WeaponChancesTable.Dice(float.Parse(p0.ToString()));
            ScenarioContext.Current.Set<int>(id, "WeaponId");
        }
               
        
        [Then(@"武器是(.*)")]
        public void Then武器是(int p0)
        {
            var weapon = ScenarioContext.Current.Get<int>("WeaponId");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(p0, weapon);
        }
    }
}
