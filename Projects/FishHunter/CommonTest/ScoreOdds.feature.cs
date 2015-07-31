// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScoreOdds.feature.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ScoreOddsFeature type.
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
	public partial class ScoreOddsFeature
	{
		private static ITestRunner testRunner;

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
		public static void FeatureSetup(TestContext testContext)
		{
			ScoreOddsFeature.testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
			var featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "ScoreOdds", 
				"In order to : 分數回饋\nAs a math : 擊中公式\nI want to be : 根據亂數表取得相對分數", ProgrammingLanguage.CSharp, (string[])(null));
			ScoreOddsFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
		public static void FeatureTearDown()
		{
			ScoreOddsFeature.testRunner.OnFeatureEnd();
			ScoreOddsFeature.testRunner = null;
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
		public virtual void TestInitialize()
		{
			if ((TechTalk.SpecFlow.FeatureContext.Current != null)
			     && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "ScoreOdds"))
			{
				GameTest.ScoreOddsFeature.FeatureSetup(null);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
		public virtual void ScenarioTearDown()
		{
			ScoreOddsFeature.testRunner.OnScenarioEnd();
		}

		public virtual void ScenarioSetup(ScenarioInfo scenarioInfo)
		{
			ScoreOddsFeature.testRunner.OnScenarioStart(scenarioInfo);
		}

		public virtual void ScenarioCleanup()
		{
			ScoreOddsFeature.testRunner.CollectScenarioErrors();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("魚的賠率為1")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ScoreOdds")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("mytag")]
		public virtual void 魚的賠率為1()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("魚的賠率為1", new[]
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
				"1", 
				"0.9"
			});
			table1.AddRow(new[]
			{
				"2", 
				"0.025"
			});
			table1.AddRow(new[]
			{
				"3", 
				"0.025"
			});
			table1.AddRow(new[]
			{
				"5", 
				"0.025"
			});
			table1.AddRow(new[]
			{
				"10", 
				"0.025"
			});
#line 8
			ScoreOddsFeature.testRunner.Given("賠率表", (string)(null), table1, "Given ");
#line 15
			ScoreOddsFeature.testRunner.When("機率是0", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 17
			ScoreOddsFeature.testRunner.Then("賠率為1", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("魚的賠率為2")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ScoreOdds")]
		public virtual void 魚的賠率為2()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("魚的賠率為2", (string[])(null));
#line 20
			this.ScenarioSetup(scenarioInfo);
#line hidden
			var table2 = new TechTalk.SpecFlow.Table(new[]
			{
				"Key", 
				"Value"
			});
			table2.AddRow(new[]
			{
				"1", 
				"0.9"
			});
			table2.AddRow(new[]
			{
				"2", 
				"0.025"
			});
			table2.AddRow(new[]
			{
				"3", 
				"0.025"
			});
			table2.AddRow(new[]
			{
				"5", 
				"0.025"
			});
			table2.AddRow(new[]
			{
				"10", 
				"0.025"
			});
#line 21
			ScoreOddsFeature.testRunner.Given("賠率表", (string)(null), table2, "Given ");
#line 28
			ScoreOddsFeature.testRunner.When("機率是0.9", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 30
			ScoreOddsFeature.testRunner.Then("賠率為2", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("魚的賠率為3")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ScoreOdds")]
		public virtual void 魚的賠率為3()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("魚的賠率為3", (string[])(null));
#line 32
			this.ScenarioSetup(scenarioInfo);
#line hidden
			var table3 = new TechTalk.SpecFlow.Table(new[]
			{
				"Key", 
				"Value"
			});
			table3.AddRow(new[]
			{
				"1", 
				"0.9"
			});
			table3.AddRow(new[]
			{
				"2", 
				"0.025"
			});
			table3.AddRow(new[]
			{
				"3", 
				"0.025"
			});
			table3.AddRow(new[]
			{
				"5", 
				"0.025"
			});
			table3.AddRow(new[]
			{
				"10", 
				"0.025"
			});
#line 33
			ScoreOddsFeature.testRunner.Given("賠率表", (string)(null), table3, "Given ");
#line 40
			ScoreOddsFeature.testRunner.When("機率是0.925", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 42
			ScoreOddsFeature.testRunner.Then("賠率為3", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("魚的賠率為5")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ScoreOdds")]
		public virtual void 魚的賠率為5()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("魚的賠率為5", (string[])(null));
#line 44
			this.ScenarioSetup(scenarioInfo);
#line hidden
			var table4 = new TechTalk.SpecFlow.Table(new[]
			{
				"Key", 
				"Value"
			});
			table4.AddRow(new[]
			{
				"1", 
				"0.9"
			});
			table4.AddRow(new[]
			{
				"2", 
				"0.025"
			});
			table4.AddRow(new[]
			{
				"3", 
				"0.025"
			});
			table4.AddRow(new[]
			{
				"5", 
				"0.025"
			});
			table4.AddRow(new[]
			{
				"10", 
				"0.025"
			});
#line 45
			ScoreOddsFeature.testRunner.Given("賠率表", (string)(null), table4, "Given ");
#line 52
			ScoreOddsFeature.testRunner.When("機率是0.950", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 54
			ScoreOddsFeature.testRunner.Then("賠率為5", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("魚的賠率為10")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ScoreOdds")]
		public virtual void 魚的賠率為10()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("魚的賠率為10", (string[])(null));
#line 56
			this.ScenarioSetup(scenarioInfo);
#line hidden
			var table5 = new TechTalk.SpecFlow.Table(new[]
			{
				"Key", 
				"Value"
			});
			table5.AddRow(new[]
			{
				"1", 
				"0.9"
			});
			table5.AddRow(new[]
			{
				"2", 
				"0.025"
			});
			table5.AddRow(new[]
			{
				"3", 
				"0.025"
			});
			table5.AddRow(new[]
			{
				"5", 
				"0.025"
			});
			table5.AddRow(new[]
			{
				"10", 
				"0.025"
			});
#line 57
			ScoreOddsFeature.testRunner.Given("賠率表", (string)(null), table5, "Given ");
#line 64
			ScoreOddsFeature.testRunner.When("機率是0.975", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 66
			ScoreOddsFeature.testRunner.Then("賠率為10", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}
	}
}

#pragma warning restore

#endregion