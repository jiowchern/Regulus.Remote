// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.feature.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the LogFeature type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Designer generated code

#region Test_Region

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TechTalk.SpecFlow;

#endregion

#pragma warning disable

namespace PureLibraryTest
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
	public partial class LogFeature
	{
		private static ITestRunner testRunner;

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
		public static void FeatureSetup(TestContext testContext)
		{
			LogFeature.testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
			var featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("zh-TW"), "Log", 
				"In Order to 寫入訊息\r\nAs a 程式物件\r\nI Want to 輸出到Dummy物件", ProgrammingLanguage.CSharp, (string[])(null));
			LogFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
		public static void FeatureTearDown()
		{
			LogFeature.testRunner.OnFeatureEnd();
			LogFeature.testRunner = null;
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
		public virtual void TestInitialize()
		{
			if ((TechTalk.SpecFlow.FeatureContext.Current != null)
			     && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "Log"))
			{
				LogFeature.FeatureSetup(null);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
		public virtual void ScenarioTearDown()
		{
			LogFeature.testRunner.OnScenarioEnd();
		}

		public virtual void ScenarioSetup(ScenarioInfo scenarioInfo)
		{
			LogFeature.testRunner.OnScenarioStart(scenarioInfo);
		}

		public virtual void ScenarioCleanup()
		{
			LogFeature.testRunner.CollectScenarioErrors();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("當訊息為Message時，印出[Info]Message")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Log")]
		public virtual void 當訊息為Message時印出InfoMessage()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("當訊息為Message時，印出[Info]Message", (string[])(null));
#line 8
			this.ScenarioSetup(scenarioInfo);
#line 9
			LogFeature.testRunner.Given("Log寫入資料是\"Message\"", (string)(null), (TechTalk.SpecFlow.Table)(null), "假設");
#line 10
			LogFeature.testRunner.When("寫入到LogInfo", (string)(null), (TechTalk.SpecFlow.Table)(null), "當");
#line 11
			LogFeature.testRunner.Then("輸出為\"[Info]Message\"", (string)(null), (TechTalk.SpecFlow.Table)(null), "那麼");
#line hidden
			this.ScenarioCleanup();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("當訊息為Message時，印出[Debug]Message")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Log")]
		public virtual void 當訊息為Message時印出DebugMessage()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("當訊息為Message時，印出[Debug]Message", (string[])(null));
#line 13
			this.ScenarioSetup(scenarioInfo);
#line 14
			LogFeature.testRunner.Given("Log寫入資料是\"Message\"", (string)(null), (TechTalk.SpecFlow.Table)(null), "假設");
#line 15
			LogFeature.testRunner.When("寫入到LogDebug", (string)(null), (TechTalk.SpecFlow.Table)(null), "當");
#line 16
			LogFeature.testRunner.Then("頭7個字元是\"[Debug]\"", (string)(null), (TechTalk.SpecFlow.Table)(null), "那麼");
#line hidden
			this.ScenarioCleanup();
		}
	}
}

#pragma warning restore

#endregion