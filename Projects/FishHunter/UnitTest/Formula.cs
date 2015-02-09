using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace VGame.Project.FishHunter.UnitTest
{
    /// <summary>
    /// Formula 的摘要描述
    /// </summary>
    [TestClass]
    public class Formula
    {
        public Formula()
        {
            //
            // TODO:  在此加入建構函式的程式碼
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///取得或設定提供目前測試回合
        ///的相關資訊與功能的測試內容。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 其他測試屬性
        //
        // 您可以使用下列其他屬性撰寫您的測試: 
        //
        // 執行該類別中第一項測試前，使用 ClassInitialize 執行程式碼
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在類別中的所有測試執行後，使用 ClassCleanup 執行程式碼
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在執行每一項測試之前，先使用 TestInitialize 執行程式碼 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在執行每一項測試之後，使用 TestCleanup 執行程式碼
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion



        [TestMethod]
        public void TestHitFormulaDeath1()
        {
            Regulus.Utility.IRandom random = NSubstitute.Substitute.For<Regulus.Utility.IRandom>();

            var formula = new VGame.Project.FishHunter.Formula.HitTest(random);
            random.NextLong(NSubstitute.Arg.Any<long>(), NSubstitute.Arg.Any<long>()).Returns(2147481);
            HitRequest request = new HitRequest();
            request.FishID = 1;
            request.FishOdds = 1;
            request.TotalHits = 1;
            request.WepBet = 1;
            request.WepOdds = 1;
            request.WepID = 1;

            HitResponse response = formula.Request(request);
            Assert.AreEqual(1, response.WepID);
            Assert.AreEqual(0, response.SpecAsn);
            Assert.AreEqual(1, response.FishID);
            Assert.AreEqual(FISH_DETERMINATION.DEATH, response.DieResult);

        }

        [TestMethod]
        public void TestHitFormulaSurvival1()
        {
            Regulus.Utility.IRandom random = NSubstitute.Substitute.For<Regulus.Utility.IRandom>();

            var formula = new VGame.Project.FishHunter.Formula.HitTest(random);
            random.NextLong(NSubstitute.Arg.Any<long>(), NSubstitute.Arg.Any<long>()).Returns(2147482);
            HitRequest request = new HitRequest();
            request.FishID = 1;
            request.FishOdds = 1;
            request.TotalHits = 1;
            request.WepBet = 1;
            request.WepOdds = 1;
            request.WepID = 1;

            HitResponse response = formula.Request(request);
            Assert.AreEqual(1, response.WepID);
            Assert.AreEqual(0, response.SpecAsn);
            Assert.AreEqual(1, response.FishID);
            Assert.AreEqual(FISH_DETERMINATION.SURVIVAL, response.DieResult);

        }


        

        
    }
}
