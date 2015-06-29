using System;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using VGame.Project.FishHunter;

namespace GameTest
{
    [Binding]
    public class ScoreOddsSteps
    {
        ScoreOddsTable _SorceOddsTable;
        
        [Given(@"賠率表")]
        public void Given賠率表(Table table)
        {
            _SorceOddsTable = new ScoreOddsTable(_ToData(table));
        }
        
        [When(@"機率是(.*)")]
        public void When機率是(float p0)
        {
            var value = _SorceOddsTable.Dice(p0);
            ScenarioContext.Current.Set<int>(value, "ScoreOdds");
        }
        
        [Then(@"賠率為(.*)")]
        public void Then賠率為(int p0)
        {
            var odds = ScenarioContext.Current.Get<int>("ScoreOdds");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(p0, odds);
        }

        private VGame.Project.FishHunter.ScoreOddsTable.Data[] _ToData(Table table)
        {
            var datas = table.CreateSet<VGame.Project.FishHunter.ScoreOddsTable.Data>();
            return datas.ToArray();                    
        }
    }
}
