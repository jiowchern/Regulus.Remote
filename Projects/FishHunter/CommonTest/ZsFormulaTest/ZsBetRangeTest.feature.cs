#region Designer generated code

using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable

namespace GameTest.ZsFormulaTest
{
	using TechTalk.SpecFlow;

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
			var featureInfo = new TechTalk.SpecFlow.FeatureInfo(
				new System.Globalization.CultureInfo("en-US"), 
				"ZsBetRangeTestSteps", 
				"In order to 計算魚場賭注\r\nAs a math 賭注分配公式\r\nI want to be 根據公式得到5個押分區間(最小~最大)", 
				ProgrammingLanguage.CSharp, 
				(string[])(null));
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
			if((TechTalk.SpecFlow.FeatureContext.Current != null)
			    && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "ZsBetRangeTestSteps"))
			{
				GameTest.ZsFormulaTest.ZsBetRangeTestStepsFeature.FeatureSetup(null);
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
#line 7
#line hidden
			var table1 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"account_id", 
					"Rate"
				});
			table1.AddRow(
				new[]
				{
					"1", 
					"0.1"
				});
			table1.AddRow(
				new[]
				{
					"2", 
					"0.25"
				});
			table1.AddRow(
				new[]
				{
					"3", 
					"0.5"
				});
			table1.AddRow(
				new[]
				{
					"4", 
					"0.75"
				});
			table1.AddRow(
				new[]
				{
					"5", 
					"1.0"
				});
#line 8
			ZsBetRangeTestStepsFeature.testRunner.Given("buffer資料是", (string)(null), table1, "Given ");
#line hidden
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得魚場押注buffer等級1")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "ZsBetRangeTestSteps")]
		public virtual void 取得魚場押注Buffer等級1()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得魚場押注buffer等級1", (string[])(null));
#line 17
			this.ScenarioSetup(scenarioInfo);
#line 7
			this.FeatureBackground();
#line 19
			ZsBetRangeTestStepsFeature.testRunner.When(
				"最大押分是1000", 
				(string)(null), 
				(TechTalk.SpecFlow.Table)(null), 
				"When ");
#line 20
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是100", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table2 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"account_id", 
					"Rate"
				});
			table2.AddRow(
				new[]
				{
					"1", 
					"0.1"
				});
#line 22
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
#line 27
			this.ScenarioSetup(scenarioInfo);
#line 7
			this.FeatureBackground();
#line 29
			ZsBetRangeTestStepsFeature.testRunner.When(
				"最大押分是1000", 
				(string)(null), 
				(TechTalk.SpecFlow.Table)(null), 
				"When ");
#line 30
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是250", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table3 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"account_id", 
					"Rate"
				});
			table3.AddRow(
				new[]
				{
					"2", 
					"0.25"
				});
#line 32
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
#line 36
			this.ScenarioSetup(scenarioInfo);
#line 7
			this.FeatureBackground();
#line 38
			ZsBetRangeTestStepsFeature.testRunner.When(
				"最大押分是1000", 
				(string)(null), 
				(TechTalk.SpecFlow.Table)(null), 
				"When ");
#line 39
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是500", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table4 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"account_id", 
					"Rate"
				});
			table4.AddRow(
				new[]
				{
					"3", 
					"0.5"
				});
#line 41
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
#line 45
			this.ScenarioSetup(scenarioInfo);
#line 7
			this.FeatureBackground();
#line 47
			ZsBetRangeTestStepsFeature.testRunner.When(
				"最大押分是1000", 
				(string)(null), 
				(TechTalk.SpecFlow.Table)(null), 
				"When ");
#line 48
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是750", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table5 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"account_id", 
					"Rate"
				});
			table5.AddRow(
				new[]
				{
					"4", 
					"0.75"
				});
#line 50
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
#line 55
			this.ScenarioSetup(scenarioInfo);
#line 7
			this.FeatureBackground();
#line 57
			ZsBetRangeTestStepsFeature.testRunner.When(
				"最大押分是1000", 
				(string)(null), 
				(TechTalk.SpecFlow.Table)(null), 
				"When ");
#line 58
			ZsBetRangeTestStepsFeature.testRunner.And("玩家押分是1000", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line hidden
			var table6 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"account_id", 
					"Rate"
				});
			table6.AddRow(
				new[]
				{
					"5", 
					"1.0"
				});
#line 60
			ZsBetRangeTestStepsFeature.testRunner.Then("取得的Buffer是", (string)(null), table6, "Then ");
#line hidden
			this.ScenarioCleanup();
		}
	}
}

#pragma warning restore

#endregion
