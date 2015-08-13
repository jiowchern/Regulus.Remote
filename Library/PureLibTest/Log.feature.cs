using System.CodeDom.Compiler;
using System.Globalization;
using System.Runtime.CompilerServices;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using TechTalk.SpecFlow;

namespace RegulusLibraryTest
{
	[GeneratedCode("TechTalk.SpecFlow", "1.9.0.77")]
	[CompilerGenerated]
	[TestClass]
	public class LogFeature
	{
		private static ITestRunner testRunner;

		[ClassInitialize]
		public static void FeatureSetup(TestContext testContext)
		{
			LogFeature.testRunner = TestRunnerManager.GetTestRunner();
			var featureInfo = new FeatureInfo(
				new CultureInfo("zh-TW"), 
				"Log", 
				"In Order to 寫入訊息\r\nAs a 程式物件\r\nI Want to 輸出到Dummy物件", 
				ProgrammingLanguage.CSharp, 
				null);
			LogFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[ClassCleanup]
		public static void FeatureTearDown()
		{
			LogFeature.testRunner.OnFeatureEnd();
			LogFeature.testRunner = null;
		}

		[TestInitialize]
		public virtual void TestInitialize()
		{
			if((FeatureContext.Current != null)
			   && (FeatureContext.Current.FeatureInfo.Title != "Log"))
			{
				LogFeature.FeatureSetup(null);
			}
		}

		[TestCleanup]
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

		[TestMethod]
		[Description("當訊息為Message時，印出[Info]Message")]
		[TestProperty("FeatureTitle", "Log")]
		public virtual void 當訊息為Message時印出InfoMessage()
		{
			var scenarioInfo = new ScenarioInfo("當訊息為Message時，印出[Info]Message", null);

			ScenarioSetup(scenarioInfo);

			LogFeature.testRunner.Given("Log寫入資料是\"Message\"", null, null, "假設");

			LogFeature.testRunner.When("寫入到LogInfo", null, null, "當");

			LogFeature.testRunner.Then("輸出為\"[Info]Message\"", null, null, "那麼");

			ScenarioCleanup();
		}

		[TestMethod]
		[Description("當訊息為Message時，印出[Debug]Message")]
		[TestProperty("FeatureTitle", "Log")]
		public virtual void 當訊息為Message時印出DebugMessage()
		{
			var scenarioInfo = new ScenarioInfo("當訊息為Message時，印出[Debug]Message", null);

			ScenarioSetup(scenarioInfo);

			LogFeature.testRunner.Given("Log寫入資料是\"Message\"", null, null, "假設");

			LogFeature.testRunner.When("寫入到LogDebug", null, null, "當");

			LogFeature.testRunner.Then("頭7個字元是\"[Debug]\"", null, null, "那麼");

			ScenarioCleanup();
		}
	}
}
