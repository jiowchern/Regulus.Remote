#region Designer generated code

using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable

namespace GameTest.ZsFormulaTest
{
	using TechTalk.SpecFlow;

	[System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
	public partial class FishHitAllocateFeature
	{
		private static ITestRunner testRunner;

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
		public static void FeatureSetup(TestContext testContext)
		{
			FishHitAllocateFeature.testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
			var featureInfo = new TechTalk.SpecFlow.FeatureInfo(
				new System.Globalization.CultureInfo("en-US"), 
				"FishHitAllocate", 
				"In order to 依擊中的魚數量，分配武器威力\r\nAs a math 分配威力表\r\nI want to be 得到命中機率", 
				ProgrammingLanguage.CSharp, 
				(string[])(null));
			FishHitAllocateFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
		public static void FeatureTearDown()
		{
			FishHitAllocateFeature.testRunner.OnFeatureEnd();
			FishHitAllocateFeature.testRunner = null;
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
		public virtual void TestInitialize()
		{
			if((TechTalk.SpecFlow.FeatureContext.Current != null)
			    && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "FishHitAllocate"))
			{
				GameTest.ZsFormulaTest.FishHitAllocateFeature.FeatureSetup(null);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
		public virtual void ScenarioTearDown()
		{
			FishHitAllocateFeature.testRunner.OnScenarioEnd();
		}

		public virtual void ScenarioSetup(ScenarioInfo scenarioInfo)
		{
			FishHitAllocateFeature.testRunner.OnScenarioStart(scenarioInfo);
		}

		public virtual void ScenarioCleanup()
		{
			FishHitAllocateFeature.testRunner.CollectScenarioErrors();
		}

		public virtual void FeatureBackground()
		{
#line 8
#line hidden
			var table1 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"HitNumber", 
					"Hit1", 
					"Hit2", 
					"Hit3", 
					"Hit4"
				});
			table1.AddRow(
				new[]
				{
					"1", 
					"1000", 
					"0", 
					"0", 
					"0"
				});
			table1.AddRow(
				new[]
				{
					"2", 
					"800", 
					"200", 
					"0", 
					"0"
				});
			table1.AddRow(
				new[]
				{
					"3", 
					"750", 
					"150", 
					"100", 
					"0"
				});
			table1.AddRow(
				new[]
				{
					"4", 
					"700", 
					"150", 
					"100", 
					"50"
				});
#line 9
			FishHitAllocateFeature.testRunner.Given("威力分配表", (string)(null), table1, "Given ");
#line hidden
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("擊中1隻魚")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FishHitAllocate")]
		public virtual void 擊中1隻魚()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("擊中1隻魚", (string[])(null));
#line 17
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 18
			FishHitAllocateFeature.testRunner.When("擊中數量是1", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line hidden
			var table2 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"HitNumber", 
					"Hit1", 
					"Hit2", 
					"Hit3", 
					"Hit4"
				});
			table2.AddRow(
				new[]
				{
					"1", 
					"1000", 
					"0", 
					"0", 
					"0"
				});
#line 19
			FishHitAllocateFeature.testRunner.Then("取出資料為", (string)(null), table2, "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("擊中2隻魚")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FishHitAllocate")]
		public virtual void 擊中2隻魚()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("擊中2隻魚", (string[])(null));
#line 23
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 24
			FishHitAllocateFeature.testRunner.When("擊中數量是2", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line hidden
			var table3 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"HitNumber", 
					"Hit1", 
					"Hit2", 
					"Hit3", 
					"Hit4"
				});
			table3.AddRow(
				new[]
				{
					"2", 
					"800", 
					"200", 
					"0", 
					"0"
				});
#line 25
			FishHitAllocateFeature.testRunner.Then("取出資料為", (string)(null), table3, "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("擊中3隻魚")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FishHitAllocate")]
		public virtual void 擊中3隻魚()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("擊中3隻魚", (string[])(null));
#line 29
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 30
			FishHitAllocateFeature.testRunner.When("擊中數量是3", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line hidden
			var table4 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"HitNumber", 
					"Hit1", 
					"Hit2", 
					"Hit3", 
					"Hit4"
				});
			table4.AddRow(
				new[]
				{
					"3", 
					"750", 
					"150", 
					"100", 
					"0"
				});
#line 31
			FishHitAllocateFeature.testRunner.Then("取出資料為", (string)(null), table4, "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("擊中4隻魚以上")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FishHitAllocate")]
		public virtual void 擊中4隻魚以上()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("擊中4隻魚以上", (string[])(null));
#line 35
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 36
			FishHitAllocateFeature.testRunner.When("擊中數量是5", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line hidden
			var table5 = new TechTalk.SpecFlow.Table(
				new[]
				{
					"HitNumber", 
					"Hit1", 
					"Hit2", 
					"Hit3", 
					"Hit4"
				});
			table5.AddRow(
				new[]
				{
					"4", 
					"700", 
					"150", 
					"100", 
					"50"
				});
#line 37
			FishHitAllocateFeature.testRunner.Then("取出資料為", (string)(null), table5, "Then ");
#line hidden
			this.ScenarioCleanup();
		}
	}
}

#pragma warning restore

#endregion
