// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleRuleSteps.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the MultipleRuleSteps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

#endregion

namespace GameTest
{
	[Binding]
	[Scope(Feature = "MultipleRule")]
	public class MultipleRuleSteps
	{
		private MultipleTable _MultipleTable;

		[Given(@"倍數表是")]
		public void Given倍數表是(Table table)
		{
			var datas = table.CreateSet<MultipleTable.Data>();

			_MultipleTable = new MultipleTable(datas);
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

			var data = _MultipleTable.Find(key);

			Assert.AreEqual(value, data.Value);
		}
	}
}