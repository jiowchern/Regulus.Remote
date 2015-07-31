// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZsFishStageDataCheckSteps.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ZsFishStageDataCheckSteps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

#endregion

namespace GameTest
{
	[Binding]
	[Scope(Feature = "ZsFishStageDataCheck")]
	public class ZsFishStageDataCheckSteps
	{
		private StageDataTable _StageDataTable;

		public ZsFishStageDataCheckSteps(StageDataTable stage_data_table)
		{
			_StageDataTable = stage_data_table;
		}

		[Given(@"魚場資料表是")]
		public void Given魚場資料表是(Table table)
		{
			var datas = table.CreateSet<StageDataTable.Data>();

			_StageDataTable = new StageDataTable(datas);
		}

		[When(@"當輸入魚場id是 (.*)")]
		public void When當輸入魚場Id是(int input_Id)
		{
			ScenarioContext.Current.Set(input_Id, "key");
		}

		[Then(@"取得的魚場資料是")]
		public void Then取得的魚場資料是(Table table)
		{
			var key = ScenarioContext.Current.Get<int>("key");

			var sourceData = _StageDataTable.TableDatas[key];

			table.CompareToInstance(sourceData);
		}
	}
}