using System.CodeDom.Compiler;
using System.Globalization;
using System.Runtime.CompilerServices;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using TechTalk.SpecFlow;

namespace GameTest.FormulaTest
{
	[Binding]
	[Scope(Feature = "WeaponChancesTable")]
	[GeneratedCode("TechTalk.SpecFlow", "1.9.0.77")]
	[CompilerGenerated]
	[TestClass]
	public class WeaponChancesTableFeature
	{
		private static ITestRunner testRunner;

		[ClassInitialize]
		public static void FeatureSetup(TestContext testContext)
		{
			WeaponChancesTableFeature.testRunner = TestRunnerManager.GetTestRunner();
			var featureInfo = new FeatureInfo(
				new CultureInfo("en-US"), 
				"WeaponChancesTable", 
				"In order to 得到特武用\r\nAs a math idiot 擊中公式 \r\nI want to be 根據亂數表取得對應武器", 
				ProgrammingLanguage.CSharp, 
				null);
			WeaponChancesTableFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[ClassCleanup]
		public static void FeatureTearDown()
		{
			WeaponChancesTableFeature.testRunner.OnFeatureEnd();
			WeaponChancesTableFeature.testRunner = null;
		}

		[TestInitialize]
		public virtual void TestInitialize()
		{
			if((FeatureContext.Current != null)
			   && (FeatureContext.Current.FeatureInfo.Title != "WeaponChancesTable"))
			{
				WeaponChancesTableFeature.FeatureSetup(null);
			}
		}

		[TestCleanup]
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

		[TestMethod]
		[Description("取得0號武器")]
		[TestProperty("FeatureTitle", "WeaponChancesTable")]
		[TestCategory("mytag")]
		public virtual void 取得0號武器()
		{
			var scenarioInfo = new ScenarioInfo("取得0號武器", "mytag");

			ScenarioSetup(scenarioInfo);

			var table1 = new Table("account_id", "Rate");
			table1.AddRow("0", "0.9");
			table1.AddRow("2", "0.033");
			table1.AddRow("3", "0.033");
			table1.AddRow("4", "0.033");

			WeaponChancesTableFeature.testRunner.Given("武器清單是", null, table1, "Given ");

			WeaponChancesTableFeature.testRunner.When("機率是\"0.899\"", null, null, "When ");

			WeaponChancesTableFeature.testRunner.Then("武器是0", null, null, "Then ");

			ScenarioCleanup();
		}

		[TestMethod]
		[Description("取得2號武器")]
		[TestProperty("FeatureTitle", "WeaponChancesTable")]
		public virtual void 取得2號武器()
		{
			var scenarioInfo = new ScenarioInfo("取得2號武器", null);

			ScenarioSetup(scenarioInfo);

			var table2 = new Table("account_id", "Rate");
			table2.AddRow("0", "0.9");
			table2.AddRow("2", "0.033");
			table2.AddRow("3", "0.033");
			table2.AddRow("4", "0.033");
			table2.AddRow("5", "999.33");

			WeaponChancesTableFeature.testRunner.Given("武器清單是", null, table2, "Given ");

			WeaponChancesTableFeature.testRunner.When("機率是\"0.93299\"", null, null, "When ");

			WeaponChancesTableFeature.testRunner.Then("武器是2", null, null, "Then ");

			ScenarioCleanup();
		}

		[TestMethod]
		[Description("取得3號武器")]
		[TestProperty("FeatureTitle", "WeaponChancesTable")]
		public virtual void 取得3號武器()
		{
			var scenarioInfo = new ScenarioInfo("取得3號武器", null);

			ScenarioSetup(scenarioInfo);

			var table3 = new Table("account_id", "Rate");
			table3.AddRow("0", "0.9");
			table3.AddRow("2", "0.033");
			table3.AddRow("3", "0.033");
			table3.AddRow("4", "0.033");

			WeaponChancesTableFeature.testRunner.Given("武器清單是", null, table3, "Given ");

			WeaponChancesTableFeature.testRunner.When("機率是\"0.965999\"", null, null, "When ");

			WeaponChancesTableFeature.testRunner.Then("武器是3", null, null, "Then ");

			ScenarioCleanup();
		}

		[TestMethod]
		[Description("取得4號武器")]
		[TestProperty("FeatureTitle", "WeaponChancesTable")]
		public virtual void 取得4號武器()
		{
			var scenarioInfo = new ScenarioInfo("取得4號武器", null);

			ScenarioSetup(scenarioInfo);

			var table4 = new Table("account_id", "Rate");
			table4.AddRow("0", "0.9");
			table4.AddRow("2", "0.033");
			table4.AddRow("3", "0.033");
			table4.AddRow("4", "0.033");

			WeaponChancesTableFeature.testRunner.Given("武器清單是", null, table4, "Given ");

			WeaponChancesTableFeature.testRunner.When("機率是\"0.966\"", null, null, "When ");

			WeaponChancesTableFeature.testRunner.Then("武器是4", null, null, "Then ");

			ScenarioCleanup();
		}

		[TestMethod]
		[Description("取得預設武器")]
		[TestProperty("FeatureTitle", "WeaponChancesTable")]
		public virtual void 取得預設武器()
		{
			var scenarioInfo = new ScenarioInfo("取得預設武器", null);

			ScenarioSetup(scenarioInfo);

			var table5 = new Table("account_id", "Rate");
			table5.AddRow("0", "0.9");
			table5.AddRow("2", "0.033");
			table5.AddRow("3", "0.033");
			table5.AddRow("4", "0.033");

			WeaponChancesTableFeature.testRunner.Given("武器清單是", null, table5, "Given ");

			WeaponChancesTableFeature.testRunner.When("機率是\"1\"", null, null, "When ");

			WeaponChancesTableFeature.testRunner.Then("武器是0", null, null, "Then ");

			ScenarioCleanup();
		}
	}
}
