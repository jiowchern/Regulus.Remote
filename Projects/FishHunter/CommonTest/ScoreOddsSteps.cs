using System;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using VGame.Project.FishHunter;
using VGame.Project.FishHunter.ZsFormula;
using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace GameTest
{
    [Binding]
	[Scope(Feature = "ScoreOdds")]
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

        private ScoreOddsTable.Data[] _ToData(Table table)
        {
            var datas = table.CreateSet<ScoreOddsTable.Data>();
            return datas.ToArray();                    
        }
    }
}
