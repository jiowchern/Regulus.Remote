
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using Regulus.Utility;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace GameTest.ZsFormulaTest
{
	[Binding]
	[Scope(Feature = "ZsFormula")]
	public class ZsFormulaSteps
	{
		private FishFarmData _FarmData;

		private RequsetFishData[] _FishDatas;

		private HitRequest _HitRequest;

		private HitResponse[] _HitResponses;

		private RequestWeaponData _WeaponData;

		/// <summary>
		///     arrange
		/// </summary>
		/// <param name="farm_id"></param>
		[Given(@"魚場編號是(.*)")]
		public void Given魚場編號是(int farm_id)
		{
			_FarmData = new FishFarmBuilder().Get(farm_id);
		}

		/// <summary>
		///     arrange
		/// </summary>
		/// <param name="table"></param>
		[Given(@"魚的資料是")]
		public void Given魚的資料是(Table table)
		{
			_FishDatas = table.CreateSet<RequsetFishData>().ToArray();
		}

		/// <summary>
		///     arrange
		/// </summary>
		/// <param name="table"></param>
		[Given(@"玩家武器資料是")]
		public void Given玩家武器資料是(Table table)
		{
			_WeaponData = table.CreateInstance<RequestWeaponData>();
		}

		private List<RandomData> _CreateRandoms()
		{
			var rs = new List<RandomData>
			{
				_CreateAdjustPlayerPhaseRandom(), 
				_CreateTreasureRandom(), 
				_CreateDeathRandom(), 
				_CreateOddsRandom()
			};

			return rs;
		}

		private RandomData _CreateAdjustPlayerPhaseRandom()
		{
			var data = new RandomData
			{
				RandomType = RandomData.RULE.ADJUSTMENT_PLAYER_PHASE, 
				Randoms = new List<IRandom>()
			};

			var r = Substitute.For<IRandom>();
			r.NextInt(0, 1000).Returns(1000);
			data.Randoms.Add(r);

			return data;
		}

		private RandomData _CreateTreasureRandom()
		{
			var data = new RandomData
			{
				RandomType = RandomData.RULE.CHECK_TREASURE, 
				Randoms = new List<IRandom>()
			};

			var r = Substitute.For<IRandom>();
			r.NextInt(0, 0x10000000).Returns(0x10000000);
			data.Randoms.Add(r);

			r = Substitute.For<IRandom>();
			r.NextFloat(0, 1).Returns(1);
			data.Randoms.Add(r);

			return data;
		}

		private RandomData _CreateDeathRandom()
		{
			var data = new RandomData
			{
				RandomType = RandomData.RULE.DEATH, 
				Randoms = new List<IRandom>()
			};

			var r = Substitute.For<IRandom>();
			r.NextInt(0, 0x10000000).Returns(0x10000000);
			data.Randoms.Add(r);

			r = Substitute.For<IRandom>();
			r.NextInt(0, 0x10000000).Returns(0x10000000);
			data.Randoms.Add(r);

			return data;
		}

		private RandomData _CreateOddsRandom()
		{
			var data = new RandomData
			{
				RandomType = RandomData.RULE.ODDS, 
				Randoms = new List<IRandom>()
			};

			var r = Substitute.For<IRandom>();
			r.NextInt(0, 1000).Returns(1000);
			data.Randoms.Add(r);

			r = Substitute.For<IRandom>();
			r.NextInt(0, 1000).Returns(1000);
			data.Randoms.Add(r);

			r = Substitute.For<IRandom>();
			r.NextInt(0, 1000).Returns(1000);
			data.Randoms.Add(r);

			r = Substitute.For<IRandom>();
			r.NextInt(0, 1000).Returns(1000);
			data.Randoms.Add(r);

			r = Substitute.For<IRandom>();
			r.NextInt(0, 1000).Returns(1000);
			data.Randoms.Add(r);

			return data;
		}

		[When(@"得到檢查結果")]
		public void When得到檢查結果()
		{
			_HitRequest = new HitRequest(_FishDatas, _WeaponData, true);
			_HitResponses = new ZsHitChecker(_FarmData, new FormulaPlayerRecord()).TotalRequest(_HitRequest);
		}

		[Then(@"比對資料")]
		public void Then比對資料()
		{
			Assert.AreEqual(1, _HitResponses[0].FishId);

			Assert.AreEqual(1, _HitResponses[0].WepId);

			//Assert.AreEqual(1, _HitResponses[0].OddsResult);

			FISH_DETERMINATION result;
			var dieResult = "DEATH";
			Enum.TryParse(dieResult, out result);
			//Assert.AreEqual(result, _HitResponses[0].DieResult);

			// WEAPON_TYPE weapon;
			// var feedback = "SUPER_BOMB";
			// Enum.TryParse(feedback, out weapon);
			// Assert.AreEqual(weapon, _HitResponses[0].FeedbackWeapons[0]);
		}
	}
}
