
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

		private RequestWeaponData _WeaponData;
		private RequsetFishData[] _FishDatas;

		private HitResponse[] _HitResponses;

		private IRandom _Random;

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
			var output = new List<string>();
			
			_Random = Substitute.For<IRandom>();
			
			_Random.NextInt(Arg.Any<int>(), Arg.Any<int>()).Returns(
				info =>
				{
					output.Add(Environment.StackTrace);
					return random_value;
				});
		}

		[When(@"得到檢查結果")]
		public void When得到檢查結果()
		{
			_HitRequest = new HitRequest(_FishDatas, _WeaponData);
			_HitResponses = new ZsHitChecker(_FarmData, new FormulaPlayerRecord(), _Random).TotalRequest(_HitRequest);
		}

		[Then(@"比對資料 fishId是(.*) WepId是(.*) DieResult是'(.*)' Feeddack是'(.*)' Wup是(.*)")]
		public void Then比對資料FishId是WepId是DieResult是Feeddack是Wup是(int fish_id, int wep_id, string die_result, string weapon_type, int wup)
		{
			Assert.AreEqual(fish_id, _HitResponses[0].FishId);
			
			Assert.AreEqual(wep_id, _HitResponses[0].WepId);

			FISH_DETERMINATION result;
			Enum.TryParse(die_result, out result);
			Assert.AreEqual(result, _HitResponses[0].DieResult);
			
			WEAPON_TYPE weapon;
			Enum.TryParse(weapon_type, out weapon);
			Assert.AreEqual(weapon, _HitResponses[0].FeedbackWeaponType[0]);

			Assert.AreEqual(wup, _HitResponses[0].WUp);
		}


	}
}
