using System.Collections.Generic;
using System.Linq;


using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;


using VGame.Project.FishHunter.Common.Data;

namespace GameTest.ZsFormulaTest
{
	[Binding]
	[Scope(Feature = "ZsFishStageDataCheck")]
	public class ZsFishStageDataCheckSteps
	{
		private Dictionary<int, FishFarmData> _Datas;

		public ZsFishStageDataCheckSteps(IEnumerable<FishFarmData> datas)
		{
		}

		[Given(@"魚場資料表是")]
		public void Given魚場資料表是(Table table)
		{
			var datas = table.CreateSet<FishFarmData>();

			_Datas = datas.ToDictionary(x => x.FarmId);
		}

		[When(@"當輸入魚場id是 (.*)")]
		public void When當輸入魚場Id是(byte input_Id)
		{
			ScenarioContext.Current.Set(input_Id, "key");
		}

		[Then(@"取得的魚場資料是")]
		public void Then取得的魚場資料是(Table table)
		{
			var key = ScenarioContext.Current.Get<byte>("key");

			var sourceData = _Datas[key];

			table.CompareToInstance(sourceData);
		}
	}
}
