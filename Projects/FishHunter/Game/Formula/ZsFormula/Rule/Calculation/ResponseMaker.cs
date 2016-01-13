using System.Collections.Generic;

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

			_Build(fish_data, FISH_DETERMINATION.DEATH);

			_Record(fish_data);
		}

		private void _Record(RequsetFishData fish_data)
		{
			var bet = _Request.WeaponData.GetTotalBet();
			var winScore = fish_data.GetRealOdds() * bet;

			new SaveDeathFishHistory(_Visitor, fish_data, _Request.WeaponData).Run();

			new SaveScoreHistory(_Visitor, winScore).Run();

			new SaveTreasure(_Visitor, fish_data).Run();
		}

		private void _Build(RequsetFishData fish_data, FISH_DETERMINATION status)
		{
			var tmp = _Visitor.GetAllTreasures();
			if(tmp.Count == 0)
			{
				tmp.Add(WEAPON_TYPE.INVALID);
			}

			_HitResponses.Add(new HitResponse
			{
				WepId = _Request.WeaponData.BulletId, 
				FishId = fish_data.FishId, 
				FishOdds = fish_data.FishOdds, 
				DieResult = status, 
				FeedbackWeapons = tmp.ToArray(), 
				WeaponBet = _Request.WeaponData.WeaponBet, 
				DieRate = fish_data.HitDieRate, 
				Multiple = fish_data.Multiple
			});
		}
	}
}
