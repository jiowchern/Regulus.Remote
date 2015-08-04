// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HitTest.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the HitTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Game;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Datas;
using VGame.Project.FishHunter.Common.GPIs;
using VGame.Project.FishHunter.ZsFormula.DataStructs;

#endregion

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
			this._Random = random;
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
			const int MAX_WEPBET = 10000;
			const int MAX_WEPODDS = 10000;
			const short MAX_TOTALHITS = 1000;
			const short MAX_FISHODDS = 1000;
			const long gateOffset = 0x0fffffff;

			if (request.WepBet > MAX_WEPBET)
			{
				return HitTest._Miss(request);
			}

			if (request.WepOdds > MAX_WEPODDS)
			{
				return HitTest._Miss(request);
			}

			if (request.TotalHits == 0 || request.TotalHits > MAX_TOTALHITS)
			{
				return HitTest._Miss(request);
			}

			if (request.FishOdds == 0 || request.FishOdds > MAX_FISHODDS)
			{
				return HitTest._Miss(request);
			}

			long gate = 1000;
			gate *= gateOffset;
			gate *= request.WepBet;
			gate /= request.TotalHits;
			gate /= request.FishOdds;
			gate /= 1000;

			if (gate > 0x0fffffff)
			{
				gate = 0x10000000;
			}

			var rValue = _Random.NextLong(0, long.MaxValue);
			var value = rValue % 0x10000000;
			if (value < gate)
			{
				return _Die(request);
			}

			return HitTest._Miss(request);
		}

		private HitResponse _Die(HitRequest request)
		{
			return new HitResponse
			{
				FishID = request.FishID, 
				DieResult = FISH_DETERMINATION.DEATH, 
				SpecAsn = (byte)_WeaponChancesTable.Dice(Random.Instance.NextFloat(0, 1)), 
				WepID = request.WepID, 
				WUp = _ScoreOddsTable.Dice(Random.Instance.NextFloat(0, 1))
			};
		}

		private static HitResponse _Miss(HitRequest request)
		{
			return new HitResponse
			{
				WepID = request.WepID, 
				DieResult = FISH_DETERMINATION.SURVIVAL, 
				FishID = request.FishID, 
				SpecAsn = 0
			};
		}
	}
}