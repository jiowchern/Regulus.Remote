﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace GameTest
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class FishHitAllocateFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "FishHitAllocate.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "FishHitAllocate", "In order to 依擊中的魚數量，分配武器威力\r\nAs a math 分配威力表\r\nI want to be 得到命中機率", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((TechTalk.SpecFlow.FeatureContext.Current != null) 
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "FishHitAllocate")))
            {
                GameTest.FishHitAllocateFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 9
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "HitNumber",
                        "Hit1",
                        "Hit2",
                        "Hit3",
                        "Hit4"});
            table1.AddRow(new string[] {
                        "1",
                        "1000",
                        "0",
                        "0",
                        "0"});
            table1.AddRow(new string[] {
                        "2",
                        "800",
                        "200",
                        "0",
                        "0"});
            table1.AddRow(new string[] {
                        "3",
                        "750",
                        "150",
                        "100",
                        "0"});
            table1.AddRow(new string[] {
                        "4",
                        "700",
                        "150",
                        "100",
                        "50"});
#line 10
 testRunner.Given("威力分配表", ((string)(null)), table1, "Given ");
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("擊中1隻魚")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FishHitAllocate")]
        public virtual void 擊中1隻魚()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("擊中1隻魚", ((string[])(null)));
#line 18
this.ScenarioSetup(scenarioInfo);
#line 9
this.FeatureBackground();
#line 19
 testRunner.When("擊中數量是1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "HitNumber",
                        "Hit1",
                        "Hit2",
                        "Hit3",
                        "Hit4"});
            table2.AddRow(new string[] {
                        "1",
                        "1000",
                        "0",
                        "0",
                        "0"});
#line 20
 testRunner.Then("取出資料為", ((string)(null)), table2, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("擊中2隻魚")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FishHitAllocate")]
        public virtual void 擊中2隻魚()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("擊中2隻魚", ((string[])(null)));
#line 24
this.ScenarioSetup(scenarioInfo);
#line 9
this.FeatureBackground();
#line 25
 testRunner.When("擊中數量是2", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "HitNumber",
                        "Hit1",
                        "Hit2",
                        "Hit3",
                        "Hit4"});
            table3.AddRow(new string[] {
                        "2",
                        "800",
                        "200",
                        "0",
                        "0"});
#line 26
 testRunner.Then("取出資料為", ((string)(null)), table3, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("擊中3隻魚")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FishHitAllocate")]
        public virtual void 擊中3隻魚()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("擊中3隻魚", ((string[])(null)));
#line 30
this.ScenarioSetup(scenarioInfo);
#line 9
this.FeatureBackground();
#line 31
 testRunner.When("擊中數量是3", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "HitNumber",
                        "Hit1",
                        "Hit2",
                        "Hit3",
                        "Hit4"});
            table4.AddRow(new string[] {
                        "3",
                        "750",
                        "150",
                        "100",
                        "0"});
#line 32
 testRunner.Then("取出資料為", ((string)(null)), table4, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("擊中4隻魚以上")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FishHitAllocate")]
        public virtual void 擊中4隻魚以上()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("擊中4隻魚以上", ((string[])(null)));
#line 36
this.ScenarioSetup(scenarioInfo);
#line 9
this.FeatureBackground();
#line 37
 testRunner.When("擊中數量是5", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "HitNumber",
                        "Hit1",
                        "Hit2",
                        "Hit3",
                        "Hit4"});
            table5.AddRow(new string[] {
                        "4",
                        "700",
                        "150",
                        "100",
                        "50"});
#line 38
 testRunner.Then("取出資料為", ((string)(null)), table5, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
