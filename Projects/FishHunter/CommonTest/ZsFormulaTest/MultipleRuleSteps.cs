using System.Linq;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;


using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace GameTest.ZsFormulaTest
{
	[Binding]
	[Scope(Feature = "MultipleRule")]
	public class MultipleRuleSteps
	{
		private OddsTable _OddsTable;

		[Given(@"倍數表是")]
		public void Given倍數表是(Table table)
		{
			var datas = table.CreateSet<OddsTable.Data>();

			_OddsTable = new OddsTable(datas);
		}

		[When(@"輸入倍數表id是(.*)")]
		public void When輸入倍數表Id是(int key)
		{
			ScenarioContext.Current.Set(key, "key");
		}

		[Then(@"得到的倍數值是(.*)")]
		public void Then得到的倍數值是(int value)
		{
			var key = ScenarioContext.Current.Get<int>("key");

			var datas = _OddsTable.Get().ToDictionary(x => x.Odds);

			var data = datas[key];

			Assert.AreEqual(value, data.Number);
		}
	}
}
