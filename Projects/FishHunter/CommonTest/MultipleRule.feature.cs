// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleRule.feature.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the MultipleRuleFeature type.
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
	public partial class MultipleRuleFeature
	{
		private static ITestRunner testRunner;

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
		public static void FeatureSetup(TestContext testContext)
		{
			MultipleRuleFeature.testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
			var featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "MultipleRule", 
				"In order to 取得翻倍的倍數\r\nAs a math 翻倍表\r\nI want to be 回傳取得的倍數", ProgrammingLanguage.CSharp, (string[])(null));
			MultipleRuleFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
		public static void FeatureTearDown()
		{
			MultipleRuleFeature.testRunner.OnFeatureEnd();
			MultipleRuleFeature.testRunner = null;
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
		public virtual void TestInitialize()
		{
			if ((TechTalk.SpecFlow.FeatureContext.Current != null)
			     && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "MultipleRule"))
			{
				GameTest.MultipleRuleFeature.FeatureSetup(null);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
		public virtual void ScenarioTearDown()
		{
			MultipleRuleFeature.testRunner.OnScenarioEnd();
		}

		public virtual void ScenarioSetup(ScenarioInfo scenarioInfo)
		{
			MultipleRuleFeature.testRunner.OnScenarioStart(scenarioInfo);
		}

		public virtual void ScenarioCleanup()
		{
			MultipleRuleFeature.testRunner.CollectScenarioErrors();
		}

		public virtual void FeatureBackground()
		{
#line 8
#line hidden
			var table1 = new TechTalk.SpecFlow.Table(new[]
			{
				"Multiple", 
				"Value"
			});
			table1.AddRow(new[]
			{
				"1", 
				"1"
			});
			table1.AddRow(new[]
			{
				"2", 
				"2"
			});
			table1.AddRow(new[]
			{
				"3", 
				"3"
			});
			table1.AddRow(new[]
			{
				"5", 
				"5"
			});
			table1.AddRow(new[]
			{
				"10", 
				"10"
			});
#line 9
			MultipleRuleFeature.testRunner.Given("倍數表是", (string)(null), table1, "Given ");
#line hidden
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得倍數1")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "MultipleRule")]
		public virtual void 取得倍數1()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得倍數1", (string[])(null));
#line 18
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 19
			MultipleRuleFeature.testRunner.When("輸入倍數表id是1", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 21
			MultipleRuleFeature.testRunner.Then("得到的倍數值是1", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得倍數2")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "MultipleRule")]
		public virtual void 取得倍數2()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得倍數2", (string[])(null));
#line 24
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 25
			MultipleRuleFeature.testRunner.When("輸入倍數表id是2", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 27
			MultipleRuleFeature.testRunner.Then("得到的倍數值是2", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得倍數3")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "MultipleRule")]
		public virtual void 取得倍數3()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得倍數3", (string[])(null));
#line 29
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 30
			MultipleRuleFeature.testRunner.When("輸入倍數表id是3", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 32
			MultipleRuleFeature.testRunner.Then("得到的倍數值是3", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得倍數5")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "MultipleRule")]
		public virtual void 取得倍數5()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得倍數5", (string[])(null));
#line 34
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 35
			MultipleRuleFeature.testRunner.When("輸入倍數表id是5", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 37
			MultipleRuleFeature.testRunner.Then("得到的倍數值是5", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("取得倍數10")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "MultipleRule")]
		public virtual void 取得倍數10()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("取得倍數10", (string[])(null));
#line 39
			this.ScenarioSetup(scenarioInfo);
#line 8
			this.FeatureBackground();
#line 40
			MultipleRuleFeature.testRunner.When("輸入倍數表id是10", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line 42
			MultipleRuleFeature.testRunner.Then("得到的倍數值是10", (string)(null), (TechTalk.SpecFlow.Table)(null), "Then ");
#line hidden
			this.ScenarioCleanup();
		}
	}
}

#pragma warning restore

#endregion