using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Formula
{
	public abstract class HitBase
	{
		public abstract HitResponse[] TotalRequest(HitRequest request);
	}
}
