// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FishHitAllocateSteps.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the FishHitAllocateSteps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

#endregion

namespace GameTest.ZsFormulaTest
{
	[Binding]
	[Scope(Feature = "FishHitAllocate")]
	public class FishHitAllocateSteps
	{
		private FishHitAllocateTable _FishHitAllocateTable;

		[Given(@"威力分配表")]
		public void Given威力分配表(Table table)
		{
			var datas = table.CreateSet<FishHitAllocateTable.Data>();

			this._FishHitAllocateTable = new FishHitAllocateTable(datas);
		}

		[When(@"擊中數量是(.*)")]
		public void When擊中數量是(int hit_number)
		{
			ScenarioContext.Current.Set(hit_number, "key");
		}

		[Then(@"取出資料為")]
		public void Then取出資料為(Table table)
		{
			var hitnumber = ScenarioContext.Current.Get<int>("key");

			var data = this._FishHitAllocateTable.GetAllocateData(hitnumber);

			table.CompareToInstance(data);
		}
	}
}