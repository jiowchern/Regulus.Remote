// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZsBetRangeTest.feature.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ZsBetRangeTestStepsFeature type.
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
	public partial class ZsBetRangeTestStepsFeature
	{
		private static ITestRunner testRunner;

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
		public static void FeatureSetup(TestContext testContext)
		{
			ZsBetRangeTestStepsFeature.testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
			var featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), 
				"ZsBetRangeTestSteps", "In order to 計算魚場賭注\r\nAs a math 賭注分配公式\r\nI want to be 根據公式得到5個押分區間(最小~最大)", 
				ProgrammingLanguage.CSharp, (string[])(null));
			ZsBetRangeTestStepsFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
		public static void FeatureTearDown()
		{
			ZsBetRangeTestStepsFeature.testRunner.OnFeatureEnd();
			ZsBetRangeTestStepsFeature.testRunner = null;
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
		public virtual void TestInitialize()
		{
			if ((TechTalk.SpecFlow.FeatureContext.Current != null)
			     && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "ZsBetRangeTestSteps"))
			{
				GameTest.ZsBetRangeTestStepsFeature.FeatureSetup(null);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
		public virtual void ScenarioTearDown()
		{
			ZsBetRangeTestStepsFeature.testRunner.OnScenarioEnd();
		}

		public virtual void ScenarioSetup(ScenarioInfo scenarioInfo)
		{
			ZsBetRangeTestStepsFeature.testRunner.OnScenarioStart(scenarioInfo);
		}

		public virtual void ScenarioCleanup()
		{
			ZsBetRangeTestStepsFeature.testRunner.CollectScenarioErrors();
		}

		public virtual void FeatureBackground()
		{
#line 8
#line hidden
			var table1 = new TechTalk.SpecFlow.Table(new[]
			{
				"Id", 
				"Rate"
			});
			table1.AddRow(new[]
			{
				"1", 
				"0.1"
			});
			table1.AddRow(new[]
			{
				"2", 
				"0.25"
			});
			table1.AddRow(new[]
			{
				"3", 
				"0.5"
			});
			table1.AddRow(new[]
			{
				"4", 
				"0.75"
			});
			table1.AddRow(new[]
			{
				"5", 
				"1.0"
			});
#line 9
			ZsBetRangeTestStepsFeature.testRunner.Given("buffer資料是", (string)(null), table1, "Given ");
#line hidden
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得魚場押注buffer等級1")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ZsBetRangeTestSteps")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("mytag")]
		public virtual void 取得魚場押注Buffer等級1()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得魚場押注buffer等級1", new[]
			{
				"mytag"
			});
#line 18
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 20
			ZsBetRangeTestStepsFeature.testRunner.When("最大押分是1000", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 21
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是100", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table2 = new TechTalk.SpecFlow.Table(new[]
			{
				"Id", 
				"Rate"
			});
			table2.AddRow(new[]
			{
				"1", 
				"0.1"
			});
#line 23
			ZsBetRangeTestStepsFeature.testRunner.Then("取得的Buffer是", (string)(null), table2, "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得魚場押注buffer等級2")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ZsBetRangeTestSteps")]
		public virtual void 取得魚場押注Buffer等級2()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得魚場押注buffer等級2", (string[])(null));
#line 28
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 30
			ZsBetRangeTestStepsFeature.testRunner.When("最大押分是1000", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 31
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是250", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table3 = new TechTalk.SpecFlow.Table(new[]
			{
				"Id", 
				"Rate"
			});
			table3.AddRow(new[]
			{
				"2", 
				"0.25"
			});
#line 33
			ZsBetRangeTestStepsFeature.testRunner.Then("取得的Buffer是", (string)(null), table3, "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得魚場押注buffer等級3")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ZsBetRangeTestSteps")]
		public virtual void 取得魚場押注Buffer等級3()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得魚場押注buffer等級3", (string[])(null));
#line 37
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 39
			ZsBetRangeTestStepsFeature.testRunner.When("最大押分是1000", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 40
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是500", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table4 = new TechTalk.SpecFlow.Table(new[]
			{
				"Id", 
				"Rate"
			});
			table4.AddRow(new[]
			{
				"3", 
				"0.5"
			});
#line 42
			ZsBetRangeTestStepsFeature.testRunner.Then("取得的Buffer是", (string)(null), table4, "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得魚場押注buffer等級4")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ZsBetRangeTestSteps")]
		public virtual void 取得魚場押注Buffer等級4()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得魚場押注buffer等級4", (string[])(null));
#line 46
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 48
			ZsBetRangeTestStepsFeature.testRunner.When("最大押分是1000", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 49
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是750", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table5 = new TechTalk.SpecFlow.Table(new[]
			{
				"Id", 
				"Rate"
			});
			table5.AddRow(new[]
			{
				"4", 
				"0.75"
			});
#line 51
			ZsBetRangeTestStepsFeature.testRunner.Then("取得的Buffer是", (string)(null), table5, "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得魚場押注buffer等級5")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ZsBetRangeTestSteps")]
		public virtual void 取得魚場押注Buffer等級5()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得魚場押注buffer等級5", (string[])(null));
#line 56
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 58
			ZsBetRangeTestStepsFeature.testRunner.When("最大押分是1000", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 59
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是1000", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table6 = new TechTalk.SpecFlow.Table(new[]
			{
				"Id", 
				"Rate"
			});
			table6.AddRow(new[]
			{
				"5", 
				"1.0"
			});
#line 61
			ZsBetRangeTestStepsFeature.testRunner.Then("取得的Buffer是", (string)(null), table6, "Then ");
#line hidden
			this.ScenarioCleanup();
		}
	}
}

#pragma warning restore

#endregion