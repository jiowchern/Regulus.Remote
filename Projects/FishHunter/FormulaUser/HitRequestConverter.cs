// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HitRequestConverter.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the HitRequestConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula
{
	internal class HitRequestConverter
	{
		private readonly IFishStage _Gpi;

		public HitRequestConverter(IFishStage gpi)
		{
			this._Gpi = gpi;
		}

		internal void Conver(short wepbet, short totalhits, short fishodds)
		{
			_Gpi.Hit(new HitRequest
			{
				WepBet = wepbet, 
				TotalHits = totalhits, 
				FishOdds = fishodds
			});
		}
	}
}