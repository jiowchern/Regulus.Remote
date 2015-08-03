// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponChancesTableSteps.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the WeaponChancesTableSteps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Regulus.Game;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using VGame.Project.FishHunter;

namespace GameTest.FormulaTest
{
	[Binding]
	[Scope(Feature = "WeaponChancesTable")]
	public class WeaponChancesTableSteps
	{
		private WeaponChancesTable _WeaponChancesTable;

		[Given(@"武器清單是")]
		public void Given武器清單是(Table table)
		{
			this._WeaponChancesTable = new WeaponChancesTable(this._ToData(table));
		}

		private ChancesTable<int>.Data[] _ToData(Table table)
		{
			var datas = table.CreateSet<ChancesTable<int>.Data>();

			return datas.ToArray();
		}

		[When(@"機率是""(.*)""")]
		public void When機率是(decimal p0)
		{
			var id = this._WeaponChancesTable.Dice(float.Parse(p0.ToString()));
			ScenarioContext.Current.Set(id, "WeaponId");
		}

		[Then(@"武器是(.*)")]
		public void Then武器是(int p0)
		{
			var weapon = ScenarioContext.Current.Get<int>("WeaponId");
			Assert.AreEqual(p0, weapon);
		}
	}
}