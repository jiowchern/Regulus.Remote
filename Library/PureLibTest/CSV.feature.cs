// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSV.feature.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the CSVFeature type.
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
	public partial class CSVFeature
	{
		private static ITestRunner testRunner;

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
		public static void FeatureSetup(TestContext testContext)
		{
			CSVFeature.testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
			var featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "CSV", 
				"In order to 解析CSV格式資料\r\nAs a 程式物件\r\nI want to 把CSV資料轉成對應物件", ProgrammingLanguage.CSharp, (string[])(null));
			CSVFeature.testRunner.OnFeatureStart(featureInfo);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
		public static void FeatureTearDown()
		{
			CSVFeature.testRunner.OnFeatureEnd();
			CSVFeature.testRunner = null;
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
		public virtual void TestInitialize()
		{
			if ((TechTalk.SpecFlow.FeatureContext.Current != null)
			     && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "CSV"))
			{
				CSVFeature.FeatureSetup(null);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
		public virtual void ScenarioTearDown()
		{
			CSVFeature.testRunner.OnScenarioEnd();
		}

		public virtual void ScenarioSetup(ScenarioInfo scenarioInfo)
		{
			CSVFeature.testRunner.OnScenarioStart(scenarioInfo);
		}

		public virtual void ScenarioCleanup()
		{
			CSVFeature.testRunner.CollectScenarioErrors();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
		[Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("讀取串流資料")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CSV")]
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("mytag")]
		public virtual void 讀取串流資料()
		{
			var scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("讀取串流資料", new[]
			{
				"mytag"
			});
#line 11
			this.ScenarioSetup(scenarioInfo);
#line 12
			CSVFeature.testRunner.Given("資料是field1,field2,field3 1,2,3 4,5,6", (string)(null), 
				(TechTalk.SpecFlow.Table)(null), "Given ");
#line 13
			CSVFeature.testRunner.And("段落符號為\" \"", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line 14
			CSVFeature.testRunner.And("分格符號為\",\"", (string)(null), (TechTalk.SpecFlow.Table)(null), "And ");
#line 15
			CSVFeature.testRunner.When("執行解析", (string)(null), (TechTalk.SpecFlow.Table)(null), "When ");
#line hidden
			var table1 = new TechTalk.SpecFlow.Table(new[]
			{
				"field1", 
				"field2", 
				"field3"
			});
			table1.AddRow(new[]
			{
				"1", 
				"2", 
				"3"
			});
			table1.AddRow(new[]
			{
				"4", 
				"5", 
				"6"
			});
#line 16
			CSVFeature.testRunner.Then("結果為", (string)(null), table1, "Then ");
#line hidden
			this.ScenarioCleanup();
		}
	}
}

#pragma warning restore

#endregion