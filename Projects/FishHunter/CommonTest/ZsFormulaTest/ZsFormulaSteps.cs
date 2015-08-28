
using System;
using System.Collections.Generic;
using System.Linq;


using Microsoft.VisualStudio.TestTools.UnitTesting;


using NSubstitute;
using NSubstitute.Routing.Handlers;


using Regulus.Extension;
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

		private RequestWeaponData _WeaponData;
		private RequsetFishData[] _FishDatas;

		private HitResponse[] _HitResponses;

		private HitRequest _HitRequest;

		/// <summary>
		/// arrange
		/// </summary>
		/// <param name="farm_id"></param>
		[Given(@"魚場編號是(.*)")]
		public void Given魚場編號是(int farm_id)
		{
			_FarmData = new FishFarmBuilder().Get(farm_id);
		}

		/// <summary>
		/// arrange
		/// </summary>
		/// <param name="table"></param>
		[Given(@"魚的資料是")]
		public void Given魚的資料是(Table table)
		{
			_FishDatas = table.CreateSet<RequsetFishData>().ToArray();
		}

		/// <summary>
		/// arrange
		/// </summary>
		/// <param name="table"></param>
		[Given(@"玩家武器資料是")]
		public void Given玩家武器資料是(Table table)
		{
			_WeaponData = table.CreateInstance<RequestWeaponData>();
		}

		/// <summary>
		/// arrange
		/// </summary>
		/// <param name="random_value"></param>
		[Given(@"亂數資料是(.*)")]
		public void Given亂數資料是(int random_value)
		{
			
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
			_HitRequest = new HitRequest(_FishDatas, _WeaponData);
			_HitResponses = new ZsHitChecker(_FarmData, new FormulaPlayerRecord(), _CreateRandoms()).TotalRequest(_HitRequest);
		}

		
		[Then(@"比對資料 fishId是1 BulletId是1 DieResult是'DEATH' Feedback是'SUPER_BOMB' OddsResult是1")]
		public void Then比對資料FishId是WepId是DieResult是Feeddack是Wup是(int fish_id, int bullet_id, string die_result, string weapon_type, int odds_result)
		{
			Assert.AreEqual(fish_id, _HitResponses[0].FishId);
			
			Assert.AreEqual(bullet_id, _HitResponses[0].WepId);

			FISH_DETERMINATION result;
			Enum.TryParse(die_result, out result);
			Assert.AreEqual(result, _HitResponses[0].DieResult);
			
			WEAPON_TYPE weapon;
			Enum.TryParse(weapon_type, out weapon);
			Assert.AreEqual(weapon, _HitResponses[0].FeedbackWeapons[0]);

			Assert.AreEqual(odds_result, _HitResponses[0].OddsResult);
		}


	}
}
