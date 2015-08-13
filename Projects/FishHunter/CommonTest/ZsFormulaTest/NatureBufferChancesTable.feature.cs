#region Designer generated code

using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable

namespace GameTest.ZsFormulaTest
{
	using TechTalk.SpecFlow;

	[System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
	public partial class NatureBufferChancesTableFeature
	{
		private static ITestRunner testRunner;

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
		public static void FeatureSetup(TestContext testContext)
		{
			NatureBufferChancesTableFeature.testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
			var featureInfo = new TechTalk.SpecFlow.FeatureInfo(
				new System.Globalization.CultureInfo("en-US"), 
				"NatureBufferChancesTable", 
				"In order to 計算自然buffer\r\nAs a math 自然buffer表\r\nI want to be 根據公式得到buffer區間(最小~最大)", 
				ProgrammingLanguage.CSharp, 
				(string[])(null));
			NatureBufferChancesTableFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
		public static void FeatureTearDown()
		{
			NatureBufferChancesTableFeature.testRunner.OnFeatureEnd();
			NatureBufferChancesTableFeature.testRunner = null;
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
		public virtual void TestInitialize()
		{
			if((TechTalk.SpecFlow.FeatureContext.Current != null)
			    && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "NatureBufferChancesTable"))
			{
				GameTest.ZsFormulaTest.NatureBufferChancesTableFeature.FeatureSetup(null);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
		public virtual void ScenarioTearDown()
		{
			NatureBufferChancesTableFeature.testRunner.OnScenarioEnd();
		}

		public virtual void ScenarioSetup(ScenarioInfo scenarioInfo)
		{
			NatureBufferChancesTableFeature.testRunner.OnScenarioStart(scenarioInfo);
		}

		public virtual void ScenarioCleanup()
		{
			NatureBufferChancesTableFeature.testRunner.CollectScenarioErrors();
		}

		public virtual void FeatureBackground()
		{
#line 8
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
					"-3", 
					"-500"
				});
			table1.AddRow(
				new[]
				{
					"-2", 
					"-150"
				});
			table1.AddRow(
				new[]
				{
					"-1", 
					"-100"
				});
			table1.AddRow(
				new[]
				{
					"0", 
					"-50"
				});
			table1.AddRow(
				new[]
				{
					"1", 
					"-30"
				});
			table1.AddRow(
				new[]
				{
					"2", 
					"0"
				});
			table1.AddRow(
				new[]
				{
					"3", 
					"20"
				});
			table1.AddRow(
				new[]
				{
					"4", 
					"50"
				});
			table1.AddRow(
				new[]
				{
					"5", 
					"100"
				});
			table1.AddRow(
				new[]
				{
					"6", 
					"150"
				});
#line 9
			NatureBufferChancesTableFeature.testRunner.Given("buffer資料是", (string)(null), table1, "Given ");
#line hidden
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得自然buffer區間")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "NatureBufferChancesTable")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("mytag")]
		public virtual void 取得自然Buffer區間()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo(
				"取得自然buffer區間", 
				new[]
				{
					"mytag"
				});
#line 23
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 24
			NatureBufferChancesTableFeature.testRunner.When(
				"基數是-3", 
				(string)(null), 
				(TechTalk.SpecFlow.Table)(null), 
				"When ");
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
					"-3", 
					"-500"
				});
#line 26
			NatureBufferChancesTableFeature.testRunner.Then("取得的Buffer是", (string)(null), table2, "Then ");
#line hidden
			this.ScenarioCleanup();
		}
	}
}

#pragma warning restore

#endregion
