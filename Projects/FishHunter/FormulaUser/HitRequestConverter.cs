using System.Collections.Generic;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula
{
	internal class HitRequestConverter
	{
		private readonly IFishStage _Gpi;

		public HitRequestConverter(IFishStage gpi)
		{
			_Gpi = gpi;
		}

		internal void Conver(int wepbet, int totalhits, int fishodds)
		{
			var weaponData = new RequestWeaponData
			{
				WeaponBet = wepbet, 
				TotalHits = totalhits
			};

			var fishDatas = new List<RequsetFishData>
			{
				new RequsetFishData()
			};

			var hitRequest = new HitRequest(fishDatas.ToArray(), weaponData, true);

			_Gpi.Hit(hitRequest);
		}
	}
}
