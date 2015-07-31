// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponChancesTable.feature.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the WeaponChancesTableFeature type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Designer generated code

#region Test_Region

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TechTalk.SpecFlow;

#endregion

#pragma warning disable

namespace GameTest
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
	public partial class WeaponChancesTableFeature
	{
		private static ITestRunner testRunner;

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
		public static void FeatureSetup(TestContext testContext)
		{
			WeaponChancesTableFeature.testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
			var featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), 
				"WeaponChancesTable", "In order to 得到特武用\r\nAs a math idiot 擊中公式 \r\nI want to be 根據亂數表取得對應武器", 
				ProgrammingLanguage.CSharp, (string[])(null));
			WeaponChancesTableFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
		public static void FeatureTearDown()
		{
			WeaponChancesTableFeature.testRunner.OnFeatureEnd();
			WeaponChancesTableFeature.testRunner = null;
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
		public virtual void TestInitialize()
		{
			if ((TechTalk.SpecFlow.FeatureContext.Current != null)
			     && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "WeaponChancesTable"))
			{
				GameTest.WeaponChancesTableFeature.FeatureSetup(null);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
		public virtual void ScenarioTearDown()
		{
			WeaponChancesTableFeature.testRunner.OnScenarioEnd();
		}

		public virtual void ScenarioSetup(ScenarioInfo scenarioInfo)
		{
			WeaponChancesTableFeature.testRunner.OnScenarioStart(scenarioInfo);
		}

		public virtual void ScenarioCleanup()
		{
			WeaponChancesTableFeature.testRunner.CollectScenarioErrors();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得0號武器")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "WeaponChancesTable")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("mytag")]
		public virtual void 取得0號武器()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得0號武器", new[]
			{
				"mytag"
			});
#line 7
			this.ScenarioSetup(scenarioInfo);
#line hidden
			var table1 = new TechTalk.SpecFlow.Table(new[]
			{
				"Key", 
				"Value"
			});
			table1.AddRow(new[]
			{
				"0", 
				"0.9"
			});
			table1.AddRow(new[]
			{
				"2", 
				"0.033"
			});
			table1.AddRow(new[]
			{
				"3", 
				"0.033"
			});
			table1.AddRow(new[]
			{
				"4", 
				"0.033"
			});
#line 8
			WeaponChancesTableFeature.testRunner.Given("武器清單是", (string)(null), table1, "Given ");
#line 14
			WeaponChancesTableFeature.testRunner.When("機率是\"0.899\"", (string)(null), (TechTalk.SpecFlow.Table)(null), 
				"When ");
#line 16
			WeaponChancesTableFeature.testRunner.Then("武器是0", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得2號武器")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "WeaponChancesTable")]
		public virtual void 取得2號武器()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得2號武器", (string[])(null));
#line 18
			this.ScenarioSetup(scenarioInfo);
#line hidden
			var table2 = new TechTalk.SpecFlow.Table(new[]
			{
				"Key", 
				"Value"
			});
			table2.AddRow(new[]
			{
				"0", 
				"0.9"
			});
			table2.AddRow(new[]
			{
				"2", 
				"0.033"
			});
			table2.AddRow(new[]
			{
				"3", 
				"0.033"
			});
			table2.AddRow(new[]
			{
				"4", 
				"0.033"
			});
			table2.AddRow(new[]
			{
				"5", 
				"999.33"
			});
#line 19
			WeaponChancesTableFeature.testRunner.Given("武器清單是", (string)(null), table2, "Given ");
#line 26
			WeaponChancesTableFeature.testRunner.When("機率是\"0.93299\"", (string)(null), (TechTalk.SpecFlow.Table)(null), 
				"When ");
#line 28
			WeaponChancesTableFeature.testRunner.Then("武器是2", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得3號武器")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "WeaponChancesTable")]
		public virtual void 取得3號武器()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得3號武器", (string[])(null));
#line 30
			this.ScenarioSetup(scenarioInfo);
#line hidden
			var table3 = new TechTalk.SpecFlow.Table(new[]
			{
				"Key", 
				"Value"
			});
			table3.AddRow(new[]
			{
				"0", 
				"0.9"
			});
			table3.AddRow(new[]
			{
				"2", 
				"0.033"
			});
			table3.AddRow(new[]
			{
				"3", 
				"0.033"
			});
			table3.AddRow(new[]
			{
				"4", 
				"0.033"
			});
#line 31
			WeaponChancesTableFeature.testRunner.Given("武器清單是", (string)(null), table3, "Given ");
#line 37
			WeaponChancesTableFeature.testRunner.When("機率是\"0.965999\"", (string)(null), (TechTalk.SpecFlow.Table)(null), 
				"When ");
#line 39
			WeaponChancesTableFeature.testRunner.Then("武器是3", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得4號武器")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "WeaponChancesTable")]
		public virtual void 取得4號武器()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得4號武器", (string[])(null));
#line 41
			this.ScenarioSetup(scenarioInfo);
#line hidden
			var table4 = new TechTalk.SpecFlow.Table(new[]
			{
				"Key", 
				"Value"
			});
			table4.AddRow(new[]
			{
				"0", 
				"0.9"
			});
			table4.AddRow(new[]
			{
				"2", 
				"0.033"
			});
			table4.AddRow(new[]
			{
				"3", 
				"0.033"
			});
			table4.AddRow(new[]
			{
				"4", 
				"0.033"
			});
#line 42
			WeaponChancesTableFeature.testRunner.Given("武器清單是", (string)(null), table4, "Given ");
#line 48
			WeaponChancesTableFeature.testRunner.When("機率是\"0.966\"", (string)(null), (TechTalk.SpecFlow.Table)(null), 
				"When ");
#line 50
			WeaponChancesTableFeature.testRunner.Then("武器是4", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得預設武器")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "WeaponChancesTable")]
		public virtual void 取得預設武器()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得預設武器", (string[])(null));
#line 52
			this.ScenarioSetup(scenarioInfo);
#line hidden
			var table5 = new TechTalk.SpecFlow.Table(new[]
			{
				"Key", 
				"Value"
			});
			table5.AddRow(new[]
			{
				"0", 
				"0.9"
			});
			table5.AddRow(new[]
			{
				"2", 
				"0.033"
			});
			table5.AddRow(new[]
			{
				"3", 
				"0.033"
			});
			table5.AddRow(new[]
			{
				"4", 
				"0.033"
			});
#line 53
			WeaponChancesTableFeature.testRunner.Given("武器清單是", (string)(null), table5, "Given ");
#line 59
			WeaponChancesTableFeature.testRunner.When("機率是\"1\"", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 61
			WeaponChancesTableFeature.testRunner.Then("武器是0", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}
	}
}

#pragma warning restore

#endregion