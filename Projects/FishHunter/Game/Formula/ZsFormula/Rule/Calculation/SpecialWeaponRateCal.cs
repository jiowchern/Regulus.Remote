using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Save;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	public class SpecialWeaponRateCal
	{
		private readonly HitRequest _Request;

		private readonly DataVisitor _Visitor;

		public SpecialWeaponRateCal(DataVisitor visitor, HitRequest request)
		{
			_Visitor = visitor;
			_Request = request;
		}

		public void Cal(RequsetFishData fish_data)
		{
			if(_IsCertainDeath(fish_data))
			{
				return;
			}

			long dieRate = new SpecialWeaponPowerTable().WeaponPowers.Find(x => x.WeaponType == _Request.WeaponData.WeaponType)
														.Power;
			dieRate *= 0x0FFFFFFF;

			dieRate /= _Request.FishDatas.Sum(x => x.GetRealOdds()); // 总倍数

			if(dieRate > 0x0FFFFFFF)
			{
				dieRate = 0x10000000; // > 100%
			}

			fish_data.HitDieRate = dieRate;
		}

		private bool _IsCertainDeath(RequsetFishData fish_data)
		{
			if(_Request.WeaponData.WeaponType != WEAPON_TYPE.BIG_OCTOPUS_BOMB && !fish_data.IsGift)
			{
				return false;
			}

			fish_data.HitDieRate = 0x10000000; // > 100% 

			// 必死不翻倍
			fish_data.IsDoubled = true;

			return true;
		}
	}
}
