// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NatureBufferChancesTableSteps.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the NatureBufferChancesTableSteps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using TechTalk.SpecFlow;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

#endregion

namespace GameTest
{
	[Binding]
	[Scope(Feature = "NatureBufferChancesTable")]
	public class NatureBufferChancesTableSteps
	{
		private NatureBufferChancesTable _NatureBufferChancesTable;

		[Given(@"buffer資料是")]
		public void GivenBuffer資料是(Table table)
		{
			ScenarioContext.Current.Pending();
		}

		[When(@"基數是(.*)")]
		public void When基數是(int p0)
		{
			ScenarioContext.Current.Pending();
		}

		[Then(@"取得的Buffer是")]
		public void Then取得的Buffer是(Table table)
		{
			ScenarioContext.Current.Pending();
		}
	}
}