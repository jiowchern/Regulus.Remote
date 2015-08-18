using Regulus.Game;
using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula
{
	public class HitTest : HitBase
	{
		private readonly IRandom _Random;

		private ScoreOddsTable _ScoreOddsTable;

		private WeaponChancesTable _WeaponChancesTable;

		public HitTest(IRandom random)
		{
			_InitialWeapon();
			_InitialScore();
			_Random = random;
		}

		private void _InitialScore()
		{
			var datas = new[]
			{
				new ChancesTable<int>.Data
				{
					Key = 1, 
					Value = 0.9f
				}, 
				new ChancesTable<int>.Data
				{
					Key = 2, 
					Value = 0.025f
				}, 
				new ChancesTable<int>.Data
				{
					Key = 3, 
					Value = 0.025f
				}, 
				new ChancesTable<int>.Data
				{
					Key = 5, 
					Value = 0.025f
				}, 
				new ChancesTable<int>.Data
				{
					Key = 10, 
					Value = 0.025f
				}
			};
			_ScoreOddsTable = new ScoreOddsTable(datas);
		}

		private void _InitialWeapon()
		{
			var datas = new[]
			{
				new ChancesTable<int>.Data
				{
					Key = 0, 
					Value = 0.9f
				}, 
				new ChancesTable<int>.Data
				{
					Key = 2, 
					Value = 0.033f
				}, 
				new ChancesTable<int>.Data
				{
					Key = 3, 
					Value = 0.033f
				}, 
				new ChancesTable<int>.Data
				{
					Key = 4, 
					Value = 0.033f
				}
			};
			_WeaponChancesTable = new WeaponChancesTable(datas);
		}

		public override HitResponse Request(HitRequest request)
		{
			foreach(var fishData in request.FishDatas)
			{
				const int MAX_WEPBET = 10000;
				const int MAX_WEPODDS = 10000;
				const short MAX_TOTALHITS = 1000;
				const short MAX_FISHODDS = 1000;
				const long gateOffset = 0x0fffffff;

				if(request.WeaponData.WepBet > MAX_WEPBET)
				{
					return HitTest._Miss(fishData, request.WeaponData);
				}

				if(request.WeaponData.WepOdds > MAX_WEPODDS)
				{
					return HitTest._Miss(fishData, request.WeaponData);
				}

				if(request.WeaponData.TotalHits == 0 || request.WeaponData.TotalHits > MAX_TOTALHITS)
				{
					return HitTest._Miss(fishData, request.WeaponData);
				}

				if(fishData.FishOdds == 0 || fishData.FishOdds > MAX_FISHODDS)
				{
					return HitTest._Miss(fishData, request.WeaponData);
				}

				long gate = 1000;
				gate *= gateOffset;
				gate *= request.WeaponData.WepBet;
				gate /= request.WeaponData.TotalHits;
				gate /= fishData.FishOdds;
				gate /= 1000;

				if(gate > 0x0fffffff)
				{
					gate = 0x10000000;
				}

				var rValue = _Random.NextLong(0, long.MaxValue);

				var value = rValue % 0x10000000;

				if(value < gate)
				{
					return _Die(fishData, request.WeaponData);
				}

				return HitTest._Miss(fishData, request.WeaponData);
			}

			return new HitResponse();
		}

		public override HitResponse[] TotalRequest(HitRequest request)
		{
			throw new System.NotImplementedException();
		}

		private HitResponse _Die(RequsetFishData fish_data, RequestWeaponData weapon_data)
		{
			return new HitResponse
			{
				WepId = weapon_data.WepId, 
				FishId = fish_data.FishId, 
				DieResult = FISH_DETERMINATION.DEATH, 
				FeedbackWeaponType = new[]
				{
					(WEAPON_TYPE)_WeaponChancesTable.Dice(Random.Instance.NextFloat(0, 1))
				}, 
				WUp = _ScoreOddsTable.Dice(Random.Instance.NextFloat(0, 1))
			};
		}

		private static HitResponse _Miss(RequsetFishData fish_data, RequestWeaponData weapon_data)
		{
			return new HitResponse
			{
				WepId = weapon_data.WepId, 
				FishId = fish_data.FishId, 
				DieResult = FISH_DETERMINATION.SURVIVAL, 
				FeedbackWeaponType = new[]
				{
					WEAPON_TYPE.INVALID
				}
			};
		}
	}
}
