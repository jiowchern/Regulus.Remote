using System;
using System.Collections.Generic;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Save;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	public class ResponseMaker
	{
		private readonly List<HitResponse> _HitResponses;

		private readonly HitRequest _Request;

		private readonly DataVisitor _Visitor;

		public ResponseMaker(DataVisitor visitor, HitRequest request)
		{
			_Visitor = visitor;
			_Request = request;
			_HitResponses = new List<HitResponse>();
		}

		public List<HitResponse> MakeAll()
		{
			foreach(var deadFish in Singleton<DeathMonitor>.Instance.DeadFishs)
			{
				_Build(deadFish, FISH_DETERMINATION.REPEAT_DEATH);
			}

			foreach(var fishData in _Request.FishDatas)
			{
				_Make(fishData);
			}

			return _HitResponses;
		}

		private void _Make(RequsetFishData fish_data)
		{
			var randomValue = _Visitor.FindIRandom(RandomData.RULE.DEATH, 1)
									.NextInt(0, 0x10000000);

			if(randomValue >= fish_data.HitDieRate)
			{
				_Miss(fish_data);
				return;
			}

			_Die(fish_data);
		}

		private void _Miss(RequsetFishData fish_data)
		{
			_Build(fish_data, FISH_DETERMINATION.SURVIVAL);
		}

		private void _Die(RequsetFishData fish_data)
		{
			new CertainTreasure(_Visitor, fish_data).Run();

			Singleton<DeathMonitor>.Instance.Record(fish_data);

			_Build(fish_data, FISH_DETERMINATION.DEATH);
		}

		private void _Build(RequsetFishData fish_data, FISH_DETERMINATION status)
		{
			_HitResponses.Add(new HitResponse
			{
				WepId = _Request.WeaponData.BulletId, 
				FishId = fish_data.FishId, 
				FishOdds = fish_data.FishOdds, 
				DieResult = status, 
				FeedbackWeapons = _Visitor.GetAllTreasures()
										.ToArray(), 
				WeaponBet = _Request.WeaponData.WeaponBet, 
				DieRate = fish_data.HitDieRate, 
				OddsResult = fish_data.OddsValue
			});
		}
	}
}
