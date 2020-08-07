using Regulus.Utility;


using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace RegulusLibraryTest
{
    [Binding]
    [Scope(Feature = "CSV")]
    public class CSVSteps
    {
        public class TestData
        {
            public string field1 { get; set; }

            public int field2 { get; set; }

            public float field3 { get; set; }
        }

        [Given(@"資料是(.*)")]
        public void Given資料是(string p0)
        {
            ScenarioContext.Current.Set(p0, "Text");
        }

        [Given(@"段落符號為""(.*)""")]
        public void Given段落符號為(string p0)
        {
            ScenarioContext.Current.Set(p0, "Paragraph");
        }

        [Given(@"分格符號為""(.*)""")]
        public void Given分格符號為(string p0)
        {
            ScenarioContext.Current.Set(p0, "Separator");
        }

        [When(@"執行解析")]
        public void When執行解析()
        {
            string text = ScenarioContext.Current.Get<string>("Text");
            string paragraph = ScenarioContext.Current.Get<string>("Paragraph");
            string separator = ScenarioContext.Current.Get<string>("Separator");

            TestData[] testDatas = CSV.Parse<TestData>(text, separator, paragraph);

            ScenarioContext.Current.Set(testDatas, "Datas");
        }

        [Then(@"結果為")]
        public void Then結果為(Table table)
        {
            TestData[] testDatas = ScenarioContext.Current.Get<TestData[]>("Datas");
            table.CompareToSet(testDatas);
        }
    }
}
