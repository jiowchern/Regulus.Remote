﻿
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

using Regulus.Utility;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula;

namespace UnitTest
{
	/// <summary>
	///     Formula 的摘要描述
	/// </summary>
	[TestClass]
	public class Formula
	{
		/// <summary>
		///     取得或設定提供目前測試回合
		///     的相關資訊與功能的測試內容。
		/// </summary>
		public TestContext TestContext { get; set; }

		#region 其他測試屬性

		// 您可以使用下列其他屬性撰寫您的測試: 
		// 執行該類別中第一項測試前，使用 ClassInitialize 執行程式碼
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		// 在類別中的所有測試執行後，使用 ClassCleanup 執行程式碼
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		// 在執行每一項測試之前，先使用 TestInitialize 執行程式碼 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		// 在執行每一項測試之後，使用 TestCleanup 執行程式碼
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		#endregion

		[TestMethod]
		public void TestHitFormulaDeath1()
		{
			var random = Substitute.For<IRandom>();

			var formula = new HitTest(random);
			random.NextLong(Arg.Any<long>(), Arg.Any<long>()).Returns(0xffffffe);

			var fishs = new List<RequsetFishData>
			{
				new RequsetFishData
				{
					FishId = 1, 
					FishOdds = 1
				}
			};
			var weapon = new RequestWeaponData
			{
				TotalHits = 1, 
				WeaponBet = 1, 
				WeaponOdds = 1, 
				BulletId = 1
			};

			var request = new HitRequest(fishs.ToArray(), weapon, true);

			var hitResponses = formula.TotalRequest(request);
			foreach(var respones in hitResponses)
			{
				Assert.AreEqual(1, respones.WepId);
				Assert.AreEqual(1, respones.FishId);
				Assert.AreEqual(FISH_DETERMINATION.DEATH, respones.DieResult);
			}
			
		}

		[TestMethod]
		public void TestHitFormulaSurvival2()
		{
			var random = Substitute.For<IRandom>();

			var formula = new HitTest(random);
			random.NextLong(Arg.Any<long>(), Arg.Any<long>()).Returns(0x0fffffff / 25);

			var fishs = new List<RequsetFishData>
			{
				new RequsetFishData
				{
					FishId = 1, 
					FishOdds = 25
				}
			};
			var weapon = new RequestWeaponData
			{
				TotalHits = 1, 
				WeaponBet = 1, 
				WeaponOdds = 1, 
				BulletId = 1
			};

			var request = new HitRequest(fishs.ToArray(), weapon, true);

			var hitResponses = formula.TotalRequest(request);
		    foreach(var response in hitResponses)
		    {
                Assert.AreEqual(1, response.WepId);
                Assert.AreEqual(WEAPON_TYPE.INVALID, response.FeedbackWeapons[0]);
                Assert.AreEqual(1, response.FishId);
                Assert.AreEqual(FISH_DETERMINATION.SURVIVAL, response.DieResult);
            }
		}

		[TestMethod]
		public void TestHitFormulaDeath2()
		{
			var random = Substitute.For<IRandom>();

			var formula = new HitTest(random);
			random.NextLong(Arg.Any<long>(), Arg.Any<long>()).Returns(0x0fffffff / 26); // 9d89d8

			var fishs = new List<RequsetFishData>
			{
				new RequsetFishData
				{
					FishId = 1, 
					FishOdds = 25
				}
			};
			var weapon = new RequestWeaponData
			{
				TotalHits = 1, 
				WeaponBet = 1, 
				WeaponOdds = 1, 
				BulletId = 1,
				WeaponType =WEAPON_TYPE.INVALID
			};

			var hitRequest = new HitRequest(fishs.ToArray(), weapon, true);

			var hitResponses = formula.TotalRequest(hitRequest); // a3d70a

		    foreach(var response in hitResponses)
		    {
                Assert.AreEqual(1, response.WepId);
                Assert.AreEqual(1, response.FishId);
                Assert.AreEqual(FISH_DETERMINATION.DEATH, response.DieResult);
            }
            
		}

		[TestMethod]
		public void TestHitFormulaSurvival1()
		{
			var random = Substitute.For<IRandom>();

			var formula = new HitTest(random);
			random.NextLong(Arg.Any<long>(), Arg.Any<long>()).Returns(0xfffffff);

			var fishs = new List<RequsetFishData>
			{
				new RequsetFishData
				{
					FishId = 1, 
					FishOdds = 1,
				}
			};
			var weapon = new RequestWeaponData
			{
				TotalHits = 1, 
				WeaponBet = 1, 
				WeaponOdds = 1, 
				BulletId = 1
			};

			var request = new HitRequest(fishs.ToArray(), weapon, true);

            var hitResponses = formula.TotalRequest(request);

		    foreach(var response in hitResponses)
		    {
                Assert.AreEqual(1, response.WepId);
                Assert.AreEqual(WEAPON_TYPE.INVALID, response.FeedbackWeapons[0]);
                Assert.AreEqual(1, response.FishId);
                Assert.AreEqual(FISH_DETERMINATION.SURVIVAL, response.DieResult);

            }
			
		}
	}
}
